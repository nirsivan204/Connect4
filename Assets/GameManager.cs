using MoonActive.Connect4;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class MyException : Exception
{
    public MyException() { }
    public MyException(string message) : base(message) { }
    public MyException(string message, Exception inner) : base(message, inner) { }
    protected MyException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
};*/


public class GameManager : MonoBehaviour
{
    const int NUM_OF_ROWS = 7;
    const int NUM_OF_COLS = 6;
    const int NUM_OF_PLAYERS = 2;
    const int NUM_OF_TOKENS_TO_CONNECT = 4;

    bool _isGameEnded = false;
    bool _isTurnOngoing = false;

    [SerializeField] ConnectGameGrid _gameBoard;
    [SerializeField] Disk[] playersDisks;

    int _turn = 0;
    IDisk _lastDiskPlaced = null;
    // Start is called before the first frame update
    private void OnEnable()
    {
        BoardManager.gameResultEvent += OnGameFinished;
        _gameBoard.ColumnClicked += TryMakeMove;
    }

    private void OnDisable()
    {
        BoardManager.gameResultEvent -= OnGameFinished;
        _gameBoard.ColumnClicked -= TryMakeMove;
    }

    private void Start()
    {
        ValidateGame();
        BoardManager.InitBoard(NUM_OF_ROWS, NUM_OF_COLS,NUM_OF_PLAYERS, NUM_OF_TOKENS_TO_CONNECT);
        _isGameEnded = false;
    }

    private void ValidateGame()
    {
        if(NUM_OF_PLAYERS < playersDisks.Length)
        {
            throw new Exception("Disks not assigned to player");
        }
        if (NUM_OF_PLAYERS > playersDisks.Length)
        {
            throw new Exception("Some players doesn't have a disk assigned to them");
        }


    }


    public void TryMakeMove(int col)
    {
        if (_isGameEnded || _isTurnOngoing)
        {
            return;
        }
        _isTurnOngoing = true;
        if (IsLegalMove(col))
        {
            MakeMove(col);
        }
    }



    private void MakeMove(int col)
    {
        Disk diskPrefab = playersDisks[_turn];
        _lastDiskPlaced = _gameBoard.Spawn(diskPrefab, col, 0);
        _lastDiskPlaced.StoppedFalling += OnDiskStoppedFalling;
        BoardManager.PutToken(col, _turn+1);
    }

    private void OnDiskStoppedFalling()
    {
        _lastDiskPlaced.StoppedFalling -= OnDiskStoppedFalling;
        FinishTurn();
    }

    private void FinishTurn()
    {
        _isTurnOngoing = false;
        _turn = (_turn + 1) % NUM_OF_PLAYERS;
    }

    private bool IsLegalMove(int col)
    {
        return BoardManager.IsEmpty(0, col);
    }

    private void OnGameFinished(GameResults result)
    {
        _isGameEnded = true;
    }

}
