using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour,IStateMachineClient
{
    [SerializeField] Text _resultText;
    [SerializeField] GameObject manu;
    const string DrawMsg = "It's a Draw";
    const string WinMsg = "Player # Wins!";

    GameMode modeChosen;
    void ValidateParams()
    {
        if (!WinMsg.Contains("#"))
        {
            throw new Exception("Winning msg should contain # in order to show the player number");
        }
    }

    private void OnValidate()
    {
        ValidateParams();
    }

    void OnEnable()
    {
        StateMachine.stateEnterEvent += OnEnterState;
        StateMachine.stateExitEvent += OnExitState;
    }

    void OnDisable()
    {
        StateMachine.stateEnterEvent -= OnEnterState;
        StateMachine.stateExitEvent -= OnExitState;
    }

    private void OnGameEnded()
    {
        GameResults result =(GameResults) PlayerPrefs.GetInt("GameResult");
        if (result == GameResults.DRAW)
        {
            ShowMsg(DrawMsg);
        }
        else
        {
            ShowMsg(WinMsg.Replace("#",((int)result).ToString()));
        }
    }

    public void OnRestartButtonPress()
    {

    }

    private void ShowMsg(string msg)
    {
        _resultText.text = msg;
        _resultText.gameObject.SetActive(true);
    }

    public void OnPVPChosen()
    {
        modeChosen = GameMode.PVP;
    }

    public void OnPVCChosen()
    {
        modeChosen = GameMode.PVC;

    }

    public void OnCVCChosen()
    {
        modeChosen = GameMode.CVC;
    }

    public void OnStartClicked()
    {
        PlayerPrefs.SetInt("GameMode", (int) modeChosen);
        StateMachine.ChangeState(GameState.GAME);
    }


    public void OnEnterState(GameState state)
    {
        switch (state)
        {
            case GameState.MANU:
                manu.SetActive(true);
                break;
            case GameState.GAME:
                manu.SetActive(false);
                break;
            case GameState.GAME_ENDED:
                OnGameEnded();
                break;
            default:
                break;
        }
    }

    public void OnExitState(GameState state)
    {
        switch (state)
        {
            case GameState.MANU:
                manu.SetActive(false);
                break;
            case GameState.GAME:
                break;
            case GameState.GAME_ENDED:
                break;
            default:
                break;
        };
    }


}
