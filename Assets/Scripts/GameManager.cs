using MoonActive.Connect4;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
    PVP,
    PVC,
    CVC
}
public class GameManager : AbstractManager
{
    #region Constants
    public const int NUM_OF_ROWS = 6; 
    public const int NUM_OF_COLS = 7;
    const int NUM_OF_PLAYERS = 2;
    const int NUM_OF_TOKENS_TO_CONNECT = 4;
    #endregion

    #region PrivateParams

    bool _isGameInProgress = false;
    bool _isTurnOngoing = false;
    /* 
    // I decided to use the serializedField attribute with dragging in the inspector for the board,
    // instead of using reference to the asset in the Resources folder for simplicity.
    // 
    // This class doesn't need many references so it is ok to use the dragging method.

    // The best way to do it will be using depenedency injection or asset bundles, but it is an overkill for this project.
    // 
    */

    [SerializeField] ConnectGameGrid _gameBoard;
    
    Disk[] _playersDisks;
    AbstractPlayer[] _players = new AbstractPlayer[NUM_OF_PLAYERS];

    int _turn = 0;
    IDisk _lastDiskPlaced = null;

    #endregion

    #region PublicParams
    public ConnectGameGrid GameBoard { get => _gameBoard; }
    #endregion

    #region GettersSetters
    public void SetPlayerTokens(IDisk[] playerTokens)
    {
        _playersDisks = (Disk[])playerTokens;
    }


    public void SetBoard(IGrid grid)
    {
        _gameBoard = (ConnectGameGrid)grid;
    }

    public IDisk GetLastDiskPlaced()
    {
        return _lastDiskPlaced;
    }
#endregion

    #region GameCommands

    /// <summary>
    /// Starts a new Game
    /// Initializing the players, com or human, and starts the turn of the first player.
    /// 
    /// </summary>
    /// <param name="mode"></param>
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
                _players[1] = new AIPlayer(this, GameData.CurrentGameDifficulty);
                break;
            case GameMode.CVC:
                _players[0] = new AIPlayer(this, GameData.CurrentGameDifficulty);
                _players[1] = new AIPlayer(this, GameData.CurrentGameDifficulty);
                break;
            default:
                throw new Exception("Illigal Game Mode");
        }
        _isGameInProgress = true;
        _players[0].EnableControls(true);
        AudioManager.Instance.PlaySound(SoundType.GameStart);
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
            _players[_turn].EnableControls(true);
        }
    }

    private void PauseGame()
    {
        _players[_turn].EnableControls(false);
    }

    private void FinishGame()
    {
        AudioManager.Instance.PlaySound(SoundType.Win);
    }

    #endregion

    #region ScriptLifeCycleFunctions


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

    /// <summary>
    /// This function runs in the editor to validate all the params of the game
    /// </summary>
    protected override void ValidateParams()
    {
        SetPlayerTokens(new Disk[] { Resources.Load<Disk>(StringsConsts.DiskBName), Resources.Load<Disk>(StringsConsts.DiskAName) });

        foreach (var Disk in _playersDisks)
        {
            if(Disk == null)
            {
                throw new Exception("Cant find reference of disks in Resources folder");
            }
        }

        if (NUM_OF_PLAYERS < _playersDisks.Length)
        {
            throw new Exception("Some disks are not assigned to player");
        }
        if (NUM_OF_PLAYERS > _playersDisks.Length)
        {
            throw new Exception("Some players doesn't have a disk assigned to them");
        }
    }
    #endregion

    #region EventHandlers
    private void OnDiskStoppedFalling()
    {
        _lastDiskPlaced.StoppedFalling -= OnDiskStoppedFalling;
        AudioManager.Instance.PlaySound(SoundType.DiskFall);
        FinishTurn();
    }
    private void OnGameResult(GameResults result)
    {
        GameData.CurrentGameResults = result;
        _isGameInProgress = false;
    }
    #endregion

    #region TurnHandlers
    /// <summary>
    /// Checks if the move is legal. If it does, it makes the move.
    /// If not, gives a chance to the player to try again.
    /// </summary>
    /// <param name="col"> column to put the token </param>
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
            //try again
            _players[_turn].EnableControls(true);
        }
    }

    /// <summary>
    ///  Puts a token in the column
    /// </summary>
    /// <param name="col">column to put the token</param>
    private void MakeMove(int col)
    {
        int row = BoardManager.FindUpperMostEmptyPlace(col);
        if (row == -1)
        {
            throw new Exception("Illigal move, column is full"); // should never get here, because checking if legal move before, but just in case
        }
        //mark that the turn is being processed
        _isTurnOngoing = true;
        Disk diskPrefab = _playersDisks[_turn];
        _lastDiskPlaced = _gameBoard.Spawn(diskPrefab, col, row);
        _lastDiskPlaced.StoppedFalling += OnDiskStoppedFalling;
        AudioManager.Instance.PlaySound(SoundType.Click);
        BoardManager.PutToken(col, _turn + 1);
    }

    /// <summary>
    /// This function is being called after each turn.
    /// If the game was finished after this turn, move to game ended state.
    /// else, start turn of the next player.
    /// 
    /// </summary>
    private void FinishTurn()
    {
        _isTurnOngoing = false;
        _players[_turn].EnableControls(false);
        if (!_isGameInProgress)
        {
            StateMachine.SetNextState(GameState.GAME_ENDED);
            return;
        }
        else
        {
            ChangeTurn();
        }
    }

    private void ChangeTurn()
    {
        _turn = (_turn + 1) % NUM_OF_PLAYERS;
        _players[_turn].EnableControls(true);
    }

    private bool IsLegalMove(int col)
    {
        return BoardManager.IsEmpty(NUM_OF_ROWS-1, col);
    }
#endregion

    #region StateMachineHandlers
    /// <summary>
    /// This Funtion handles entering to a state
    /// </summary>
    /// <param name="state">The state to enter</param>
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
                    StartGame(GameData.CurrentGameMode);
                }
                break;
            case GameState.GAME_ENDED:
                FinishGame();
                break;
            case GameState.PAUSE:
                PauseGame();
                break;
            case GameState.RESTART:
                RestartGame();
                break;
            default:
                break;
        };
    }



    /// <summary>
    /// This Funtion handles exiting from a state
    /// </summary>
    /// <param name="state">the state to exit</param>
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

    #endregion

}
