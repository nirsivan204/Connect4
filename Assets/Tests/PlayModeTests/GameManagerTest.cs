using System;
using System.Collections;
using System.Collections.Generic;
using MoonActive.Connect4;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameManagerTest
{
    private GameManager testObject;
    private IDisk _lastDisk;
    private bool _isStoppedFalling = false;

    private class GameManagerMock: GameManager
    {
        public new const int NUM_OF_ROWS = 6;
        public new const int NUM_OF_COLS = 7;
        protected override void ValidateParams()
        {
            return;
        }

    }
    private class AudioManagerMock: AudioManager
    {
        public override void PlaySound(SoundType soundType, bool isLoop = false)
        {
            return;
        }

    }


    GameManager CreateGameManager()
    {
        GameManagerMock GM = GameObject.Instantiate(new GameObject()).AddComponent<GameManagerMock>();

        GM.SetBoard(GameObject.Instantiate(Resources.Load<GameObject>(StringsConsts.BoardName)).GetComponentInChildren<ConnectGameGrid>());
        GM.SetPlayerTokens(new Disk[2] { Resources.Load<GameObject>(StringsConsts.DiskAName).GetComponentInChildren<Disk>(), Resources.Load<GameObject>(StringsConsts.DiskBName).GetComponentInChildren<Disk>() });

        return GM;
    }

    [SetUp]

    public void SetupScene()
    {
        GameObject.Instantiate(new GameObject().AddComponent<AudioManagerMock>());

    }


    [UnityTest]
    //This test will check an example game, where player1 wins.
    //It checks turn switch, and basic game functionality
    public IEnumerator GameManagerTestWithEnumeratorPasses()
    {
        testObject = CreateGameManager();
        StateMachine.SetNextState(GameState.GAME);
        yield return null;
        MakeMove(0);
        yield return new WaitUntil(IsStoppedFalling);
        MakeMove(1);
        yield return new WaitUntil(IsStoppedFalling);
        MakeMove(0);
        yield return new WaitUntil(IsStoppedFalling);
        MakeMove(1);
        yield return new WaitUntil(IsStoppedFalling);
        MakeMove(0);
        yield return new WaitUntil(IsStoppedFalling);
        MakeMove(1);
        yield return new WaitUntil(IsStoppedFalling);
        MakeMove(0);
        yield return new WaitUntil(IsStoppedFalling);
        Assert.AreEqual(GameState.GAME_ENDED, StateMachine.currentState);
    }

    private void MakeMove(int col)
    {
        testObject.TryMakeMove(col);
        _isStoppedFalling = false;
        _lastDisk = testObject.GetLastDiskPlaced();
        _lastDisk.StoppedFalling += OnDiskStoppedFalling;
    }

    private void OnDiskStoppedFalling()
    {
        _lastDisk.StoppedFalling -= OnDiskStoppedFalling;
        _isStoppedFalling = true;


    }

    private bool IsStoppedFalling()
    {
        return _isStoppedFalling;
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(testObject.GameBoard);
        GameObject.Destroy(testObject);
        GameObject.Destroy(AudioManager.Instance);
    }
}
