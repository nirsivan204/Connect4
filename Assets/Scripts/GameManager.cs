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

public enum GameMode
{
    PVP,
    PVC,
    CVC
}
public class GameManager : MonoBehaviour,IStateMachineClient
{
    public const int NUM_OF_ROWS = 6; 
    public const int NUM_OF_COLS = 7;
    const int NUM_OF_PLAYERS = 2;
    const int NUM_OF_TOKENS_TO_CONNECT = 4;

    bool _isGameEnded = false;
    bool _isTurnOngoing = false;

    [SerializeField] ConnectGameGrid _gameBoard;
    [SerializeField] Disk[] playersDisks;
    AbstractPlayer[] Players = new AbstractPlayer[NUM_OF_PLAYERS];

    int _turn = 0;
    IDisk _lastDiskPlaced = null;

    public ConnectGameGrid GameBoard { get => _gameBoard; }

    // Start is called before the first frame update
    private void OnEnable()
    {
        BoardManager.gameResultEvent += OnGameFinished;
        StateMachine.stateEnterEvent += OnEnterState;
        StateMachine.stateExitEvent += OnExitState;
    }

    private void OnDisable()
    {
        BoardManager.gameResultEvent -= OnGameFinished;
        StateMachine.stateEnterEvent -= OnEnterState;
        StateMachine.stateExitEvent -= OnExitState;
    }

    private void OnValidate()
    {
        ValidateGame();
    }

    public void StartGame(GameMode mode)
    {
        BoardManager.InitBoard(NUM_OF_ROWS, NUM_OF_COLS, NUM_OF_PLAYERS, NUM_OF_TOKENS_TO_CONNECT);
        switch (mode)
        {
            case GameMode.PVP:
                Players[0] = new HumanPlayer(this);
                Players[1] = new HumanPlayer(this);
                break;
            case GameMode.PVC:
                Players[0] = new HumanPlayer(this);
                Players[1] = new AIPlayer(this,AIPlayer.Difficult.EASY);
                break;
            case GameMode.CVC:
                Players[0] = new AIPlayer(this, AIPlayer.Difficult.EASY);
                Players[1] = new AIPlayer(this, AIPlayer.Difficult.EASY);
                break;
            default:
                throw new Exception("Illigal Game Mode");
        }
        _isGameEnded = false;
        Players[0].StartTurn();
    }

    private void ValidateGame()
    {
        if(NUM_OF_PLAYERS < playersDisks.Length)
        {
            throw new Exception("Some disks are not assigned to player");
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
        if (IsLegalMove(col))
        {
            MakeMove(col);
        }
        else
        {
            Players[_turn].StartTurn();
        }
    }



    private void MakeMove(int col)
    {
        _isTurnOngoing = true;
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
        Players[_turn].EndTurn();
        _turn = (_turn + 1) % NUM_OF_PLAYERS;
        Players[_turn].StartTurn();
    }

    private bool IsLegalMove(int col)
    {
        return BoardManager.IsEmpty(0, col);
    }

    private void OnGameFinished(GameResults result)
    {
        _isGameEnded = true;
        PlayerPrefs.SetInt("GameResult", (int)result);
        StateMachine.ChangeState(GameState.GAME_ENDED);
    }

    public void OnEnterState(GameState state)
    {
        switch (state)
        {
            case GameState.MANU:
                _gameBoard.gameObject.SetActive(false);
                break;
            case GameState.GAME:
                _gameBoard.gameObject.SetActive(true);
                StartGame((GameMode)PlayerPrefs.GetInt("GameMode"));
                break;
            case GameState.GAME_ENDED:
                _isGameEnded = true;
                break;
            default:
                break;
        };
    }

    public void OnExitState(GameState state)
    {
        switch (state)
        {
            case GameState.MANU:
                break;
            case GameState.GAME:
                _gameBoard.gameObject.SetActive(false);
                break;
            case GameState.GAME_ENDED:
                break;
            default:
                break;
        };
    }
}
