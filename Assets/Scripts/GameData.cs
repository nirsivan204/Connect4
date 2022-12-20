using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    public static GameMode CurrentGameMode { get; set; }
    public static GameResults CurrentGameResults { get; set; }
    public static Difficult CurrentGameDifficulty { get; set; } = Difficult.EASY;

}
