﻿using SRTPluginProviderRE1.Structs.GameStructs;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace SRTPluginProviderRE1
{
    public struct GameMemoryRE1 : IGameMemoryRE1
    {
        private const string IGT_TIMESPAN_STRING_FORMAT = @"hh\:mm\:ss";
        public string GameName => "RE1R";
        public string VersionInfo => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public GamePlayer Player { get => _player; set => _player = value; }
        internal GamePlayer _player;

        public string PlayerName => string.Format("{0}: ", mStartPlayer.ToString());

        public int mGameMode { get => _mGameMode; set => _mGameMode = value; }
        internal int _mGameMode;

        public int mDifficulty { get => _mDifficulty; set => _mDifficulty = value; }
        internal int _mDifficulty;

        public CharacterEnumeration mStartPlayer { get => (CharacterEnumeration)_mStartPlayer; set => _mStartPlayer = (int)value; }
        internal int _mStartPlayer;

        public int mCostumeID { get => _mCostumeID; set => _mCostumeID = value; }
        internal int _mCostumeID;

        public CharacterEnumeration mChangePlayerID { get => (CharacterEnumeration)_mChangePlayerID; set => _mChangePlayerID = (int)value; }
        internal int _mChangePlayerID;

        public int mDisplayMode { get => _mDisplayMode; set => _mDisplayMode = value; }
        internal int _mDisplayMode;

        public int mVoiceType { get => _mVoiceType; set => _mVoiceType = value; }
        internal int _mVoiceType;

        public int mShadowQuality { get => _mShadowQuality; set => _mShadowQuality = value; }
        internal int _mShadowQuality;

        public int mIsSubWepAuto { get => _mIsSubWepAuto; set => _mIsSubWepAuto = value; }
        internal int _mIsSubWepAuto;

        public float mFrameCounter { get => _mFrameCounter; set => _mFrameCounter = value; }
        internal float _mFrameCounter;

        public float mPlayTime { get => _mPlayTime; set => _mPlayTime = value; }
        internal float _mPlayTime;

        public byte mIsStartGame { get => _mIsStartGame; set => _mIsStartGame = value; }
        internal byte _mIsStartGame;

        public byte mIsLoadGame { get => _mIsLoadGame; set => _mIsLoadGame = value; }
        internal byte _mIsLoadGame;

        public GameInventoryEntry[] Inventory { get => _inventory; set => _inventory = value; }
        internal GameInventoryEntry[] _inventory;

        public GameEnemyHP[] EnemyHealth { get => _enemyHealth; set => _enemyHealth = value; }
        internal GameEnemyHP[] _enemyHealth;

        public TimeSpan IGTTimeSpan
        {
            get
            {
                TimeSpan timespanIGT;

                if (mPlayTime >= 0f)
                    timespanIGT = TimeSpan.FromSeconds(mPlayTime);
                else
                    timespanIGT = new TimeSpan();

                return timespanIGT;
            }
        }

        public string IGTFormattedString => IGTTimeSpan.ToString(IGT_TIMESPAN_STRING_FORMAT, CultureInfo.InvariantCulture);
    }
}