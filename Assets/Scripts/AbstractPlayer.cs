using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPlayer
{
    #region PrivateParameters
    protected bool _canPlay;
    protected GameManager _gameMgr;
    #endregion
    public AbstractPlayer(GameManager gameMgr)
    {
        _gameMgr = gameMgr;
    }

    /// <summary>
    /// This function sets if the player can make a move or no
    /// 
    /// </summary>
    /// <param name="isEnabled"> Can the player move</param>
    public virtual void EnableControls(bool isEnabled)
    {
        _canPlay = isEnabled;
    }
    /// <summary>
    /// This function makes the move the player decided
    /// </summary>
    /// <param name="col">The column to put the token</param>
    public virtual void MakeTurn(int col)
    {
        if (_canPlay)
        {
            _gameMgr.TryMakeMove(col);
        }
    }
}
