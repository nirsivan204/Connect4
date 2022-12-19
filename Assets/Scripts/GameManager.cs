using MoonActive.Connect4;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
public class GameManager : AbstractManager
{
    public const int NUM_OF_ROWS = 6; 
    public const int NUM_OF_COLS = 7;
    const int NUM_OF_PLAYERS = 2;
    const int NUM_OF_TOKENS_TO_CONNECT = 4;

    bool _isGameInProgress = false;
    bool _isTurnOngoing = false;

    [SerializeField] ConnectGameGrid _gameBoard;
    [SerializeField] Disk[] _playersDisks;
    AbstractPlayer[] _players = new AbstractPlayer[NUM_OF_PLAYERS];

    int _turn = 0;
    IDisk _lastDiskPlaced = null;

    public ConnectGameGrid GameBoard { get => _gameBoard; }


    // Start is called before the first frame update
    protected override void OnEnable()
    {
        BoardManager.gameResultEvent += OnGameResult;
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        BoardManager.gameResultEvent -= OnGameResult;
        base.OnDisable();
    }

    public void StartGame(GameMode mode)
    {
        BoardManager.InitBoard(NUM_OF_ROWS, NUM_OF_COLS, NUM_OF_PLAYERS, NUM_OF_TOKENS_TO_CONNECT);
        switch (mode)
        {
            case GameMode.PVP:
                _players[0] = new HumanPlayer(this);
                _players[1] = new HumanPlayer(this);
                break;
            case GameMode.PVC:
                _players[0] = new HumanPlayer(this);
                _players[1] = new AIPlayer(this,AIPlayer.Difficult.EASY);
                break;
            case GameMode.CVC:
                _players[0] = new AIPlayer(this, AIPlayer.Difficult.EASY);
                _players[1] = new AIPlayer(this, AIPlayer.Difficult.EASY);
                break;
            default:
                throw new Exception("Illigal Game Mode");
        }
        _isGameInProgress = true;
        _players[0].StartTurn();
        AudioManager.Instance.PlaySound(SoundType.GameStart);
    }

    protected override void ValidateParams()
    {
        if(NUM_OF_PLAYERS < _playersDisks.Length)
        {
            throw new Exception("Some disks are not assigned to player");
        }
        if (NUM_OF_PLAYERS > _playersDisks.Length)
        {
            throw new Exception("Some players doesn't have a disk assigned to them");
        }
    }


    public void TryMakeMove(int col)
    {
        if (!_isGameInProgress || _isTurnOngoing)
        {
            return;
        }
        if (IsLegalMove(col))
        {
            MakeMove(col);
        }
        else
        {
            _players[_turn].StartTurn();
        }
    }



    private void MakeMove(int col)
    {
        int row = BoardManager.FindUpperMostEmptyPlace(col);
        if(row == -1) 
        {
            return; // should never get here, because checking if legal move before
        }
        _isTurnOngoing = true;
        Disk diskPrefab = _playersDisks[_turn];

        _lastDiskPlaced = _gameBoard.Spawn(diskPrefab, col, row);
        _lastDiskPlaced.StoppedFalling += OnDiskStoppedFalling;
        AudioManager.Instance.PlaySound(SoundType.Click);
        BoardManager.PutToken(col, _turn+1);
    }

    private void OnDiskStoppedFalling()
    {
        _lastDiskPlaced.StoppedFalling -= OnDiskStoppedFalling;
        AudioManager.Instance.PlaySound(SoundType.DiskFall);
        FinishTurn();
    }

    private void FinishTurn()
    {
        _isTurnOngoing = false;
        ChangeTurn();
    }

    private bool IsLegalMove(int col)
    {
        return BoardManager.IsEmpty(NUM_OF_ROWS-1, col);
    }

    private void OnGameResult(GameResults result)
    {
        PlayerPrefs.SetInt(StringsConsts.PPResult, (int)result);
        _isGameInProgress = false;
    }

    private void OnGameFinished()
    {
        AudioManager.Instance.PlaySound(SoundType.Win);
    }

    public override void OnEnterState(GameState state)
    {
        switch (state)
        {
            case GameState.MANU:
                AudioManager.Instance.PlaySound(SoundType.BG_Music,true);
                break;
            case GameState.GAME:
                _gameBoard.gameObject.SetActive(true);
                if (!_isGameInProgress)
                {
                    StartGame((GameMode)PlayerPrefs.GetInt(StringsConsts.PPGameMode));
                }
                break;
            case GameState.GAME_ENDED:
                OnGameFinished();
                break;
            case GameState.PAUSE:
                OnPause();
                break;
            case GameState.RESTART:
                RestartGame();
                break;
            default:
                break;
        };
    }


    private void RestartGame()
    {
        _isGameInProgress = false;
        SceneManager.LoadScene(StringsConsts.GameSceneName);
    }

    private void ResumeGame()
    {
        if (_isGameInProgress)
        {
            _players[_turn].StartTurn();
        }
    }

    public override void OnExitState(GameState state)
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
            case GameState.PAUSE:
                ResumeGame();
                break;
            case GameState.RESTART:
                break;
            default:
                break;
        };
    }

    private void ChangeTurn()
    {
        _players[_turn].EndTurn();
        if (!_isGameInProgress)
        {
            StateMachine.SetNextState(GameState.GAME_ENDED);
            return;
        }
        _turn = (_turn + 1) % NUM_OF_PLAYERS;
        _players[_turn].StartTurn();
    }

    private void OnPause()
    {
        _players[_turn].EndTurn();
    }

}
