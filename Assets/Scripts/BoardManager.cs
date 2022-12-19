using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameResults
{
    DRAW,
    PLAYER1WIN,
    PLAYER2WIN,
}

public static class BoardManager
{

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


    static int[,] _gameBoard; // At first i thought to use enum as the type, because there are only 2 players but this is more open to extensions (3 or more players)

    public static event Action<GameResults> gameResultEvent;

    static int _tokensPlaced;
    static int _rows;
    static int _cols;
    static int _numOfPlayers;
    static int _tokensToConnect;
    static GameResults? _result = null;

    public static int TokensPlaced { get => _tokensPlaced;  }
    public static GameResults? Result { get => _result;  }

    public static void InitBoard(int rows, int cols)
    {
        InitBoard(rows, cols, 2, 4);
    }


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

    public static int GetToken(int row, int col)
    {
        return _gameBoard[row, col];
    }

    static int GetToken(TokenPlace place)
    {
        return GetToken(place.row, place.col);
    }

    static void SetToken(int row, int col, int playerID)
    {
        if (_gameBoard[row, col] != 0)
        {
            throw new Exception("Not an empty space, can't put this token here");
        }
        _gameBoard[row, col] = playerID;
        _tokensPlaced++;
        CheckResult(new TokenPlace(row, col), playerID);
    }

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
        int row = FindUpperMostEmptyPlace(col);
        if(row == -1)
        {
            throw new Exception("Tried putting player token in a full column");
        }
        else
        {
            SetToken( row, col, playerID);
        }
    }

    public static int FindUpperMostEmptyPlace(int col)
    {
        for (int i = 0; i < _rows; i++)
        {
            if (IsEmpty(i,col))
            {
                return i;
            }

        }
        return -1; //No empty Place in col
    }

    private static bool IsWinInRow(TokenPlace place)
    {
        int token = GetToken(place);
        int count = 1;
        for (int i = 1; i < _tokensToConnect && place.col+i < _cols; i++)
        {
            if(GetToken(place.row, place.col+i) == token)
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

    public static bool IsEmpty(int row, int col)
    {
        return _gameBoard[row,col] == 0;
    }

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


    private static void CheckResult(TokenPlace lastTokenPlaced, int playerID)
    {
        if(IsWinInRow(lastTokenPlaced) || IsWinInCol(lastTokenPlaced) || IsWinLeftDiagonal(lastTokenPlaced) || IsWinRightDiagonal(lastTokenPlaced))
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
}
