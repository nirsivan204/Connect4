using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used for saving strings that are being used across the scripts of the game. without this, a change in one script will require changes in other scripts. 
public static class StringsConsts
{
    #region SceneNames
    public const string GameSceneName = "Connect4_Game";
    #endregion
    #region Assets
    public const string DiskAName = "Disk_A";
    public const string DiskBName = "Disk_B";
    public const string BoardName =  "Connect4Board";

    #endregion
    #region PlayerPrefsNames
    public const string PPMusicVolName = "MusicVol";
    public const string PPSFXVolName = "SFXVol";
    #endregion
}
