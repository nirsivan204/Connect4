using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour
{
    public enum Difficult
    {
        EASY,
        MEDIUM,
        HARD,
    }

    Difficult _difficultLevel;
    delegate int CalculateNextMoveFunc(int[,] board);
    CalculateNextMoveFunc _nextMoveFunc;

    public AIPlayer(Difficult level)
    {
        _difficultLevel = level;
        switch (level)
        {
            case Difficult.EASY:
                _nextMoveFunc = Easy;
                break;
            case Difficult.MEDIUM:
                _nextMoveFunc = Medium;
                break;
            case Difficult.HARD:
                _nextMoveFunc = Hard;
                break;
            default:
                break;

        }
    }



    int Easy(int[,] board)
    {
        return Random.Range(0, board.GetLength(1)); 
    }

    int Medium(int[,] board)
    {
        throw new System.NotImplementedException();

    }

    private int Hard(int[,] board)
    {
        throw new System.NotImplementedException();
    }

}
