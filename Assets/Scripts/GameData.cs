using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static GameMode CurrentGameMode { get; set; }
    public static GameResults CurrentGameResults { get; set; }
    public static Difficult CurrentGameDifficulty { get; set; } = Difficult.EASY;
    
    public static void SavePlayerMusicPrefs(float musicVol, float sfxVol)
    {
        PlayerPrefs.SetFloat(StringsConsts.PPMusicVolName, musicVol);
        PlayerPrefs.SetFloat(StringsConsts.PPSFXVolName, sfxVol);
    }

    public static void LoadPlayerMusicPrefs(out float musicVol,out float sfxVol)
    {
        musicVol = PlayerPrefs.GetFloat(StringsConsts.PPMusicVolName,1); 
        sfxVol = PlayerPrefs.GetFloat(StringsConsts.PPSFXVolName,1);
    }
}
