using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : AbstractManager
{
    [SerializeField] Text _resultText;
    [SerializeField] GameObject mainManu;
    [SerializeField] GameObject manuButton;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject pauseManu;
    
    const string DrawMsg = "It's a Draw";
    const string WinMsg = "Player # Wins!";

    GameMode modeChosen;
    protected override void ValidateParams()
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

    private void OnGameEnded()
    {
        GameResults result =(GameResults) PlayerPrefs.GetInt(StringsConsts.PPResult);
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
        StateMachine.SetNextState(GameState.RESTART);
    }

    public void OnExitPausePress()
    {
        StateMachine.SetNextState(GameState.GAME);
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
        PlayerPrefs.SetInt(StringsConsts.PPGameMode, (int) modeChosen);
        if(StateMachine.currentState == GameState.PAUSE)
        {
            StateMachine.SetNextState(GameState.RESTART);
        }
        else
        {
            if(StateMachine.currentState == GameState.MANU)
            {
                StateMachine.SetNextState(GameState.GAME);
            }
        }
    }


    public override void OnEnterState(GameState state)
    {
        switch (state)
        {
            case GameState.MANU:
                mainManu.SetActive(true);
                break;
            case GameState.GAME:
                mainManu.SetActive(false);
                restartButton.SetActive(true);
                manuButton.SetActive(true);
                break;
            case GameState.GAME_ENDED:
                restartButton.SetActive(true);
                manuButton.SetActive(true);
                OnGameEnded();
                break;
            case GameState.PAUSE:
                pauseManu.SetActive(true);
                restartButton.SetActive(false);
                manuButton.SetActive(false);
                break;
            case GameState.RESTART:
                break;
            default:
                break;
        }
    }

    public void OnOpenManuClick()
    {
        if (StateMachine.currentState == GameState.GAME)
        {
            StateMachine.SetNextState(GameState.PAUSE);

        }
        else
        {
            if(StateMachine.currentState == GameState.GAME_ENDED)
            {
                StateMachine.SetNextState(GameState.MANU);
            }
        }
    }

    public void OnMainManuClicked()
    {
        StateMachine.SetNextState(GameState.MANU);
    }

    public override void OnExitState(GameState state)
    {
        switch (state)
        {
            case GameState.MANU:
                mainManu.SetActive(false);
                break;
            case GameState.GAME:
                break;
            case GameState.GAME_ENDED:
                _resultText.gameObject.SetActive(false);
                break;
            case GameState.PAUSE:
                pauseManu.SetActive(false);
                break;
            case GameState.RESTART:
                break;
            default:
                break;
        };
    }


}
