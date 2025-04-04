using ProcessMemory;
using System;
using System.Diagnostics;
using SRTPluginProviderRE1.Structs.GameStructs;
using Windows.Win32.System.ProcessStatus;
using Windows.Win32;

namespace SRTPluginProviderRE1
{
    internal unsafe class GameMemoryRE1Scanner : IDisposable
    {
        private readonly int MAX_ITEMS = 10;
        private readonly int MAX_ENTITIES = 48 - 1;

        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE1 gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess is not null && memoryAccess.ProcessRunning;
        public uint ProcessExitCode => (memoryAccess is not null) ? memoryAccess.ProcessExitCode : 0U;

        // Pointer Address Variables
        private nint pointerGameState;
        private nint pointerAddressHP;

        // Pointer Classes
        private FreeLibrarySafeHandle BaseAddress { get; set; }
        private MultilevelPointer PointerGameState { get; set; }
        private MultilevelPointer PointerPlayerHP { get; set; }
        private MultilevelPointer[] PointerEntities { get; set; }

        private GameInventoryEntry EmptySlot = new GameInventoryEntry();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proc"></param>
        internal GameMemoryRE1Scanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE1();
            if (process != null)
                Initialize(process);
        }

        internal void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            if (!SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName)))
                return; // Unknown version.

            uint pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = new FreeLibrarySafeHandle(process.MainModule.BaseAddress, false);
                // Broken, might be missing something for 32-bit support. Access violation error upon execution of this line.
                //BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, ENUM_PROCESS_MODULES_EX_FLAGS.LIST_MODULES_32BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.
                {
                    nint baseAddressNIntPtr = BaseAddress.DangerousGetHandle();

                    // GET GAMEPLAY
                    PointerGameState = new MultilevelPointer(memoryAccess, (nint*)(baseAddressNIntPtr + pointerGameState));

                    // GET PLAYER
                    PointerPlayerHP = new MultilevelPointer(
                        memoryAccess,
                        (nint*)(baseAddressNIntPtr + pointerAddressHP),
                        0x14C
                    );

                    // GET ENEMIES
                    PointerEntities = new MultilevelPointer[MAX_ENTITIES];
                    gameMemoryValues._enemyHealth = new GameEnemyHP[MAX_ENTITIES];
                    for (int i = 0; i < PointerEntities.Length; ++i)
                    {
                        gameMemoryValues._enemyHealth[i] = new GameEnemyHP();
                        PointerEntities[i] = new MultilevelPointer(
                            memoryAccess,
                            (nint*)(baseAddressNIntPtr + pointerAddressHP),
                            ((i + 1) * 0x8) + 0x14C
                        );
                    }

                    // GET ITEMS
                    gameMemoryValues._inventory = new GameInventoryEntry[MAX_ITEMS];
                    for (int i = 0; i < gameMemoryValues._inventory.Length; ++i)
                        gameMemoryValues._inventory[i] = new GameInventoryEntry();
                }
            }
        }

        private bool SelectPointerAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.WW_20250205_1:
                case GameVersion.WW_20230801_1:
                case GameVersion.WW_20181019_1:
                    {
                        pointerGameState = 0x0097C9C0;
                        pointerAddressHP = 0x009E41BC;
                        return true;
                    }
            }

            // If we made it this far... rest in pepperonis. We have failed to detect any of the correct versions we support and have no idea what pointer addresses to use. Bail out.
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdatePointers()
        {
            PointerGameState.UpdatePointers();
            PointerPlayerHP.UpdatePointers();
            for (int i = 0; i < PointerEntities.Length; ++i)
                PointerEntities[i].UpdatePointers();
        }

        internal IGameMemoryRE1 Refresh()
        {
            gameMemoryValues._player = PointerPlayerHP.Deref<GamePlayer>(0x13BC);

            gameMemoryValues._mGameMode = PointerGameState.DerefInt(0x20);
            gameMemoryValues._mDifficulty = PointerGameState.DerefInt(0x24);
            gameMemoryValues._mStartPlayer = PointerGameState.DerefInt(0x5110);
            gameMemoryValues._mCostumeID = PointerGameState.DerefInt(0x5114);
            gameMemoryValues._mChangePlayerID = PointerGameState.DerefInt(0x5118);
            gameMemoryValues._mDisplayMode = PointerGameState.DerefInt(0x511C);
            gameMemoryValues._mVoiceType = PointerGameState.DerefInt(0x5120);
            gameMemoryValues._mShadowQuality = PointerGameState.DerefInt(0x5104);
            gameMemoryValues._mIsSubWepAuto = PointerGameState.DerefInt(0x5128);
            gameMemoryValues._mFrameCounter = PointerGameState.DerefFloat(0xE4738);
            gameMemoryValues._mPlayTime = PointerGameState.DerefFloat(0xE474C);
            gameMemoryValues._mIsStartGame = PointerGameState.DerefByte(0xE477E);
            gameMemoryValues._mIsLoadGame = PointerGameState.DerefByte(0xE477C);

            for (var i = 0; i < ((gameMemoryValues.mStartPlayer == CharacterEnumeration.Jill) ? 10 : 8); i++)
            {
                if (gameMemoryValues.mStartPlayer != CharacterEnumeration.Jill && i > 8)
                {
                    gameMemoryValues._inventory[i] = EmptySlot;
                }
                gameMemoryValues._inventory[i] = PointerGameState.Deref<GameInventoryEntry>(0x38 + (i * 0x8));
            }

            for (int i = 0; i < PointerEntities.Length; ++i)
                gameMemoryValues._enemyHealth[i] = PointerEntities[i].Deref<GameEnemyHP>(0x13BC);

            HasScanned = true;
            return gameMemoryValues;
        }

        private uint? GetProcessId(Process process) => (uint?)process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private unsafe bool SafeReadByteArray(nuint address, nuint size, out byte[] readBytes)
        {
            readBytes = new byte[size];
            fixed (byte* p = readBytes)
                return memoryAccess.TryGetByteArrayAt(address, size, p);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}