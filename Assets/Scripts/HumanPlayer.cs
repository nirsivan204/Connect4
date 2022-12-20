using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : AbstractPlayer
{
    public HumanPlayer(GameManager gameMgr): base(gameMgr)
    {
        //Subscribe to ColumnClicked event
        _gameMgr.GameBoard.ColumnClicked += MakeTurn;
    }

    ~HumanPlayer()
    {
        //clean event listener when class instance is destroyed
        _gameMgr.GameBoard.ColumnClicked -= MakeTurn;
    }

}
