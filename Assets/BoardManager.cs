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
    const int TOKENS_TO_WIN = 4;

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


    static int?[,] _gameBoard; // At first i thought to use enum. but this is more open to extensions

    public static event Action<GameResults> gameResultEvent;

    static int _tokensPlaced;
    static int _rows;
    static int _cols;

    public static void InitBoard(int rows,int cols)
    {
        _rows = rows;
        _cols = cols;
        _gameBoard = new int?[rows,cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; i < cols; i++)
            {
                _gameBoard[i, j] = null;
            }
        }
        _tokensPlaced = 0;
    }

    public static int? GetToken(int row, int col)
    {
        return _gameBoard[row, col];
    }

    static int? GetToken(TokenPlace place)
    {
        return GetToken(place.row, place.col);
    }

    static void SetToken(int row, int col, int playerID)
    {
        if(_gameBoard[row, col] != null)
        {
            Debug.LogError("Not an empty space, can't put this token here");
        }
        _gameBoard[row, col] = playerID;
        CheckResult(new TokenPlace(row, col));
    }

    public static bool PutToken(int col, int playerID)
    {
        for (int i = _rows-1; i >= 0; i--)
        {
            if(GetToken(i,col) == null)
            {
                SetToken(i, col, playerID);
                return true;
            }
        }
        return false;
    }




    private static bool IsWinInRow(TokenPlace place)
    {
        int? token = GetToken(place);
        int count = 1;
        for (int i = 1; i < TOKENS_TO_WIN && place.col+i < _cols; i++)
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
        for (int i = 1; i < TOKENS_TO_WIN && place.col - i >= 0; i++)
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
        return count >= TOKENS_TO_WIN;
    }

    public static bool IsEmpty(int row, int col)
    {
        return _gameBoard[row,col] == null;
    }

    private static bool IsWinInCol(TokenPlace place)
    {
        int? token = GetToken(place);
        int count = 1;
        for (int i = 1; i < TOKENS_TO_WIN && place.row + i < _rows; i++)
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
        for (int i = 1; i < TOKENS_TO_WIN && place.row - i >= 0; i++)
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
        return count >= TOKENS_TO_WIN;
    }

    private static bool IsWinLeftDiagonal(TokenPlace place)
    {
        int? token = GetToken(place);
        int count = 1;
        for (int i = 1; i < TOKENS_TO_WIN && place.row + i < _rows && place.col + i < _cols; i++)
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
        for (int i = 1; i < TOKENS_TO_WIN && place.col - i >= 0 && place.row - i >= 0; i++)
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
        return count >= TOKENS_TO_WIN;
    }


    private static bool IsWinRightDiagonal(TokenPlace place)
    {
        int? token = GetToken(place);
        int count = 1;
        for (int i = 1; i < TOKENS_TO_WIN && place.row + i < _rows && place.col - i >= 0; i++)
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
        for (int i = 1; i < TOKENS_TO_WIN && place.col + i < _cols && place.row - i >= 0; i++)
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
        return count >= TOKENS_TO_WIN;
    }


    private static void CheckResult(TokenPlace lastTokenPlaced)
    {
        if(IsWinInRow(lastTokenPlaced) || IsWinInCol(lastTokenPlaced) || IsWinLeftDiagonal(lastTokenPlaced) || IsWinRightDiagonal(lastTokenPlaced))
        {
            gameResultEvent.Invoke((GameResults) GetToken(lastTokenPlaced));
        }
        else
        {
            if (_tokensPlaced == _rows * _cols)
            {
                gameResultEvent.Invoke(GameResults.DRAW);
            }
        }
    }
}
