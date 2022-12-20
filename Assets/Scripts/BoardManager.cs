using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region Enums
public enum GameResults
{
    DRAW,
    PLAYER1WIN,
    PLAYER2WIN,
}
#endregion
public static class BoardManager
{
    #region PrivateStructs
    private struct TokenPlace
    {
        public int row;
        public int col;

        public TokenPlace(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
    #endregion

    #region Events
    public static event Action<GameResults> gameResultEvent;
    #endregion

    #region PrivateParams
    static int[,] _gameBoard; // At first i thought to use enum as the type of the array, because there are only 2 players but this is more open to extensions (3 or more players)
    static int _tokensPlaced;
    static int _rows;
    static int _cols;
    static int _numOfPlayers;
    static int _tokensToConnect;
    static GameResults? _result = null;
    #endregion

    #region PublicParams
    public static int TokensPlaced { get => _tokensPlaced;  }
    public static GameResults? Result { get => _result;  }

    #endregion

    #region PublicAPI
    /// <summary>
    /// Initalizes a new board
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="cols"></param>
    public static void InitBoard(int rows, int cols)
    {
        InitBoard(rows, cols, 2, 4);
    }
    /// <summary>
    /// Initalizes a new board
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="cols"></param>
    /// <param name="numOfPlayers"></param>
    /// <param name="tokensToConnect"></param>
    public static void InitBoard(int rows, int cols, int numOfPlayers, int tokensToConnect)
    {
        _rows = rows;
        _cols = cols;
        _numOfPlayers = numOfPlayers;
        _tokensToConnect = tokensToConnect;
        _result = null;
        _gameBoard = new int[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                _gameBoard[i, j] = 0;
            }
        }
        _tokensPlaced = 0;
    }
    /// <summary>
    /// Returns the player id of the token in this row and column
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns>player ID</returns>
    public static int GetToken(int row, int col)
    {
        CheckRowAndColInBounds(row, col);
        return _gameBoard[row, col];
    }
    /// <summary>
    /// puts a token of player ID inside the coloumn
    /// </summary>
    /// <param name="col"></param>
    /// <param name="playerID"></param>
    public static void PutToken(int col, int playerID)
    {
        if (playerID <= 0 || playerID > _numOfPlayers)
        {
            throw new Exception("Tried putting illigal player token");
        }
        if (_result != null)
        {
            throw new Exception("Game already has a result, can't put more tokens");
        }
        CheckColInBounds(col);
        int row = FindUpperMostEmptyPlace(col);
        if (row == -1)
        {
            throw new Exception("Tried putting player token in a full column");
        }
        else
        {
            SetToken(row, col, playerID);
        }
    }

    /// <summary>
    /// Returns uppermost empty place in column 
    /// </summary>
    /// <param name="col"></param>
    /// <returns>row of empty place, or -1 of column is full</returns>
    public static int FindUpperMostEmptyPlace(int col)
    {
        CheckColInBounds(col);
        for (int i = 0; i < _rows; i++)
        {
            if (IsEmpty(i, col))
            {
                return i;
            }

        }
        return -1; //No empty Place in col
    }

    /// <summary>
    /// Returns is a place in the board is empty
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns>true if empty, else false</returns>
    public static bool IsEmpty(int row, int col)
    {
        CheckRowAndColInBounds(row, col);
        return _gameBoard[row, col] == 0;
    }
    #endregion

    #region PrivateMethods
    /// <summary>
    /// returns the playerID from  a specific place in board
    /// 
    /// </summary>
    /// <param name="place"></param>
    /// <returns></returns>
    static int GetToken(TokenPlace place)
    {
        return GetToken(place.row, place.col);
    }
    /// <summary>
    /// sets a token in board
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="playerID"></param>
    static void SetToken(int row, int col, int playerID)
    {
        if (!IsEmpty(row, col))
        {
            throw new Exception("Not an empty space, can't put this token here");
        }
        _gameBoard[row, col] = playerID;
        _tokensPlaced++;
        CheckResult(new TokenPlace(row, col), playerID);
    }
    /// <summary>
    /// Checks in conditions after token is placed. If player won, or board is full, invokes gameResultEvent with the game result.
    /// </summary>
    /// <param name="lastTokenPlaced"></param>
    /// <param name="playerID"> Who played</param>
    private static void CheckResult(TokenPlace lastTokenPlaced, int playerID)
    {
        if (IsWinInRow(lastTokenPlaced) || IsWinInCol(lastTokenPlaced) || IsWinLeftDiagonal(lastTokenPlaced) || IsWinRightDiagonal(lastTokenPlaced))
        {
            gameResultEvent?.Invoke((GameResults)playerID);
            _result = (GameResults)playerID;
        }
        else
        {
            if (_tokensPlaced == _rows * _cols)
            {
                gameResultEvent?.Invoke(GameResults.DRAW);
                _result = GameResults.DRAW;
            }
        }
    }
    #endregion

    #region InputValidators
    private static void CheckRowAndColInBounds(int row, int col)
    {
        CheckRowInBounds(row);
        CheckColInBounds(col);
    }

    private static void CheckRowInBounds(int row)
    {
        if (row >= _rows || row < 0)
        {
            throw new Exception("row outside of board bounds");
        }
    }

    private static void CheckColInBounds(int col)
    {
        if (col >= _cols || col < 0)
        {
            throw new Exception("col outside of board bounds");
        }
    }

    #endregion

    #region WinConditions

    /// <summary>
    /// checks if there is a win in row
    /// </summary>
    /// <param name="place">place of token placed</param>
    /// <returns>true if win, else false </returns>
    private static bool IsWinInRow(TokenPlace place)
    {
        int token = GetToken(place);
        int count = 1;
        for (int i = 1; i < _tokensToConnect && place.col + i < _cols; i++)
        {
            if (GetToken(place.row, place.col + i) == token)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = 1; i < _tokensToConnect && place.col - i >= 0; i++)
        {
            if (GetToken(place.row, place.col - i) == token)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        return count >= _tokensToConnect;
    }


    /// <summary>
    /// checks if there is a win in col
    /// </summary>
    /// <param name="place">place of token placed</param>
    /// <returns>true if win, else false </returns>
    private static bool IsWinInCol(TokenPlace place)
    {
        int token = GetToken(place);
        int count = 1;
        for (int i = 1; i < _tokensToConnect && place.row + i < _rows; i++)
        {
            if (GetToken(place.row + i, place.col) == token)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = 1; i < _tokensToConnect && place.row - i >= 0; i++)
        {
            if (GetToken(place.row - i, place.col) == token)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        return count >= _tokensToConnect;
    }
    /// <summary>
    /// checks if there is a win in left diagonal
    /// </summary>
    /// <param name="place">place of token placed</param>
    /// <returns>true if win, else false </returns>
    private static bool IsWinLeftDiagonal(TokenPlace place)
    {
        int token = GetToken(place);
        int count = 1;
        for (int i = 1; i < _tokensToConnect && place.row + i < _rows && place.col + i < _cols; i++)
        {
            if (GetToken(place.row + i, place.col + i) == token)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = 1; i < _tokensToConnect && place.col - i >= 0 && place.row - i >= 0; i++)
        {
            if (GetToken(place.row - i, place.col - i) == token)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        return count >= _tokensToConnect;
    }

    /// <summary>
    /// checks if there is a win in Right diagonal
    /// </summary>
    /// <param name="place">place of token placed</param>
    /// <returns>true if win, else false </returns>
    private static bool IsWinRightDiagonal(TokenPlace place)
    {
        int token = GetToken(place);
        int count = 1;
        for (int i = 1; i < _tokensToConnect && place.row + i < _rows && place.col - i >= 0; i++)
        {
            if (GetToken(place.row + i, place.col - i) == token)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = 1; i < _tokensToConnect && place.col + i < _cols && place.row - i >= 0; i++)
        {
            if (GetToken(place.row - i, place.col + i) == token)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        return count >= _tokensToConnect;
    }
    #endregion



}
