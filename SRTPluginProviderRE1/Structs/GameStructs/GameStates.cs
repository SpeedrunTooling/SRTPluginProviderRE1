using System.Runtime.InteropServices;

namespace SRTPluginProviderRE1.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]

    public unsafe struct GameStates
    {
        [FieldOffset(0x20)] public int GameMode;
        [FieldOffset(0x24)] public int Difficulty;
        [FieldOffset(0x5104)] public int ShadowQuality;
        [FieldOffset(0x5110)] public int StartPlayer;
        [FieldOffset(0x5114)] public int CostumeID;
        [FieldOffset(0x5118)] public int ChangePlayerID;
        [FieldOffset(0x511C)] public int DisplayMode;
        [FieldOffset(0x5120)] public int VoiceType;
        [FieldOffset(0x5128)] public int IsSubWepAuto;
        [FieldOffset(0xE4738)] public float FrameCounter;
        [FieldOffset(0xE474C)] public float PlayTime;
        [FieldOffset(0xE477E)] public byte IsStartGame;
        [FieldOffset(0xE477C)] public byte IsLoadGame;

        public static GameStates AsStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(GameStates*)pb;
            }
        }
    }
}