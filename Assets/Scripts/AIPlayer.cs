using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : AbstractPlayer
{
    public enum Difficult
    {
        EASY,
        MEDIUM,
        HARD,
    }

    delegate int CalculateNextMoveFunc(int[,] board = null);
    CalculateNextMoveFunc _nextMoveFunc;

    public AIPlayer(GameManager gameMgr, Difficult level) : base(gameMgr)
    {
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

    public override void StartTurn()
    {
        base.StartTurn();
        MakeTurn(_nextMoveFunc());
    }


    int Easy(int[,] board)
    {
        return Random.Range(0, GameManager.NUM_OF_COLS); 
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
