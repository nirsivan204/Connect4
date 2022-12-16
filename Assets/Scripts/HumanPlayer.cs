using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : AbstractPlayer
{
    public HumanPlayer(GameManager gameMgr): base(gameMgr)
    {
        _gameMgr.GameBoard.ColumnClicked += MakeTurn;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _gameMgr.GameBoard.ColumnClicked -= MakeTurn;
    }

}
