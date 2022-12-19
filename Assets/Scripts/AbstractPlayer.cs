using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPlayer
{
    protected bool _canPlay;
    protected GameManager _gameMgr;

    public AbstractPlayer(GameManager gameMgr)
    {
        _gameMgr = gameMgr;
    }

    public virtual void OnDestroy()
    {

    }

    public virtual void EnableControls(bool isEnabled)
    {
        _canPlay = isEnabled;
    }

    public virtual void MakeTurn(int col)
    {
        if (_canPlay)
        {
            _gameMgr.TryMakeMove(col);
        }
    }
}
