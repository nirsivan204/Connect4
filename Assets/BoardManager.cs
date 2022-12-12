using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameResults
{
    PLAYER1WIN,
    PLAYER2WIN,
    DRAW
}

public enum TokensType
{
    EMPTY,
    PLAYER1,
    PLAYER2,
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


    static TokensType[,] _gameBoard;

    static event Action<GameResults> gameResultEvent;

    static int _tokensPlaced;
    static int _rows;
    static int _cols;

    public static void InitBoard(int rows,int cols)
    {
        _rows = rows;
        _cols = cols;
        _gameBoard = new TokensType[rows,cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; i < cols; i++)
            {
                _gameBoard[i, j] = TokensType.EMPTY;
            }
        }
        _tokensPlaced = 0;
    }

    static TokensType GetToken(int row, int col)
    {
        return _gameBoard[row, col];
    }

    static TokensType GetToken(TokenPlace place)
    {
        return GetToken(place.row, place.col);
    }

    static void SetToken(int row, int col, TokensType type)
    {
        if(_gameBoard[row, col] != TokensType.EMPTY)
        {
            Debug.LogError("Not an empty space, can't put this token here");
        }
        _gameBoard[row, col] = type;
        CheckResult(new TokenPlace(row, col));
    }

    public static bool PutToken(int col, TokensType type)
    {
        for (int i = _rows-1; i >= 0; i--)
        {
            if(GetToken(i,col) == TokensType.EMPTY)
            {
                SetToken(i, col, type);
                return true;
            }
        }
        return false;
    }




    private static bool IsWinInRow(TokenPlace place)
    {
        TokensType token = GetToken(place);
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

    private static bool IsWinInCol(TokenPlace place)
    {
        TokensType token = GetToken(place);
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
        TokensType token = GetToken(place);
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
        TokensType token = GetToken(place);
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
            if(GetToken(lastTokenPlaced) == TokensType.PLAYER1)
            {
                gameResultEvent.Invoke(GameResults.PLAYER1WIN);

            }
            else
            {
                gameResultEvent.Invoke(GameResults.PLAYER2WIN);
            }
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
