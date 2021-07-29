using SRTPluginProviderRE1.Structs.GameStructs;
using System;

namespace SRTPluginProviderRE1
{
    public interface IGameMemoryRE1
    {
        // Raw data properties.
        string GameName { get; }
        string VersionInfo { get; }
        GamePlayer Player { get; set; }
        int mGameMode { get; set; }
        int mDifficulty { get; set; }
        CharacterEnumeration mStartPlayer { get; set; } // Selected Character
        int mCostumeID { get; set; }
        CharacterEnumeration mChangePlayerID { get; set; } // Current Character
        int mDisplayMode { get; set; }
        int mVoiceType { get; set; }
        int mShadowQuality { get; set; }
        int mIsSubWepAuto { get; set; }
        float mFrameCounter { get; set; }
        float mPlayTime { get; set; } // IGT
        byte mIsStartGame { get; set; }
        byte mIsLoadGame { get; set; }
        GameInventoryEntry[] Inventory { get; set; }
        GameEnemyHP[] EnemyHealth { get; set; }

        // Calculated properties.
        TimeSpan IGTTimeSpan { get; }
        string IGTFormattedString { get; }
    }
}