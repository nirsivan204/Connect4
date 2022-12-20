using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BoardManagerTests
{

    [Test]
    public void WinTest()
    {
    int[,] winTestBoardVertical = new int[4, 4] {
    { 0,1,0,0 },
    { 2,1,0,0 },
    { 2,1,0,0 },
    { 2,1,0,0 } };

    int[,] winTestBoardHorizontal = new int[4, 4] {
    { 0,0,0,0 },
    { 1,1,1,1 },
    { 2,2,2,1 },
    { 1,1,1,2 } };

    int[,] winTestBoardRightDiagonal = new int[4, 4] {
    { 1,0,1,2 },
    { 1,0,2,1 },
    { 1,2,2,1 },
    { 2,2,1,1 } };

    int[,] winTestBoardLeftDiagonal = new int[4, 4] {
    { 2,0,0,0 },
    { 1,2,2,0 },
    { 1,2,2,1 },
    { 1,1,1,2 } };


        MakeBoardFromMatrix(4, 4, winTestBoardVertical);
        Assert.AreEqual(GameResults.PLAYER1WIN, BoardManager.Result);

        MakeBoardFromMatrix(4, 4, winTestBoardHorizontal);
        Assert.AreEqual(GameResults.PLAYER1WIN, BoardManager.Result);

        MakeBoardFromMatrix(4, 4, winTestBoardRightDiagonal);
        Assert.AreEqual(GameResults.PLAYER2WIN, BoardManager.Result);

        MakeBoardFromMatrix(4, 4, winTestBoardLeftDiagonal);
        Assert.AreEqual(GameResults.PLAYER2WIN, BoardManager.Result);

    }

    [Test]
    public void DrawTest()
    {
    int[,] drawTestBoard1 = new int[4, 4] {
    { 1,2,1,1 },
    { 2,1,2,1 },
    { 2,1,2,1 },
    { 2,1,2,2 } };

    int[,] drawTestBoard2 = new int[4, 4] {
    { 1,2,2,1 },
    { 1,2,1,2 },
    { 2,2,2,1 },
    { 1,1,1,2 } };


        MakeBoardFromMatrix(4, 4, drawTestBoard1);
        Assert.AreEqual(GameResults.DRAW, BoardManager.Result);

        MakeBoardFromMatrix(4, 4, drawTestBoard2);
        Assert.AreEqual(GameResults.DRAW, BoardManager.Result);

    }



    [Test]
    public void BoardValidationCheck()
    {
        //no tokens check
        MakeBoardUsingTurns(4, 5, new int[] { });
        Assert.AreEqual(0, BoardManager.TokensPlaced);

        //validate count of tokens
        MakeBoardUsingTurns(4, 5, new int[] { 0, 1, 2, 3, 4 });
        Assert.AreEqual(5,BoardManager.TokensPlaced);
        MakeBoardUsingTurns(2, 2, new int[] { 0, 1, 0, 1});
        Assert.AreEqual(4, BoardManager.TokensPlaced);

        //validate put and get
        MakeBoardUsingTurns(4, 5, new int[] { 0, 0, 0, 0 });
        Assert.AreEqual(2, BoardManager.GetToken(3, 0));
        Assert.AreEqual(1, BoardManager.GetToken(2, 0));

        //validate isEmpty
        MakeBoardUsingTurns(4, 5, new int[] { 0, 0, 0, 0 });
        Assert.IsTrue(BoardManager.IsEmpty(1, 1));
        Assert.IsFalse(BoardManager.IsEmpty(2, 0));

        //validate assertion for illigal move
        MakeBoardUsingTurns(4, 5, new int[] { 0, 0, 0, 0 });
        Assert.Throws<Exception>(()=>BoardManager.PutToken(0,1));

        //validate assertion for illigal players move
        Assert.Throws<Exception>(() => BoardManager.PutToken(1, 0));
        Assert.Throws<Exception>(() => BoardManager.PutToken(1, -1));
        Assert.Throws<Exception>(() => BoardManager.PutToken(1, 5));


    }

    private void MakeBoardUsingTurns(int rows, int cols, int[] turnsArray)
    {
        BoardManager.InitBoard(rows, cols);
        for (int i = 0; i < turnsArray.Length; i++)
        {
            BoardManager.PutToken(turnsArray[i],i%2+1);
        }
    }

    private void MakeBoardFromMatrix(int rows, int cols, int[,] boardMatrix)
    {
        if (boardMatrix.GetLength(0) != rows || boardMatrix.GetLength(1) != cols)
        {
            throw new Exception("Error in test case");
        }
        BoardManager.InitBoard(rows, cols);
        for (int i = rows-1; i >= 0; i--)
        {
            for (int j = 0; j < cols; j++)
            {
                if(boardMatrix[i, j] != 0)
                {
                    BoardManager.PutToken(j, boardMatrix[i, j]);
                }
            }
        }
    }


}
