using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficult
{
    EASY,
    MEDIUM,
    HARD,
}

public class AIPlayer : AbstractPlayer
{
    //Delegate for functions of different strategies. Takes the current state of the board, and Returns the col of the next move chosen.
    delegate int CalculateNextMoveFunc(int[,] board = null);
    CalculateNextMoveFunc _nextMoveFunc;
/// <summary>
/// Builds an AI player and sets it's difficult.
/// </summary>
/// <param name="gameMgr"> The game manager</param>
/// <param name="level"> The difficult level</param>
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

    public override void EnableControls(bool isEnabled)
    {
        base.EnableControls(isEnabled);
        if (isEnabled)
        {
            MakeTurn(_nextMoveFunc());
        }
    }


    int Easy(int[,] board)
    {
        return Random.Range(0, GameManager.NUM_OF_COLS); 
    }

    int Medium(int[,] board)
    {
        //todo: random until going to Lose/Win in the next move
        throw new System.NotImplementedException();

    }

    int Hard(int[,] board)
    {
        //todo: min max algorithem
        throw new System.NotImplementedException();
    }


}
