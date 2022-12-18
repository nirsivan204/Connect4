using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPlayer
{
    protected bool isMyTurn;
    protected GameManager _gameMgr;

    public AbstractPlayer(GameManager gameMgr)
    {
        _gameMgr = gameMgr;
    }

    public virtual void OnDestroy()
    {

    }


    public void EndTurn()
    {
        isMyTurn = false;

    }

    public virtual void StartTurn()
    {
        isMyTurn = true;
    }

    public virtual void MakeTurn(int col)
    {
        if (isMyTurn)
        {
            _gameMgr.TryMakeMove(col);
        }
    }
}
