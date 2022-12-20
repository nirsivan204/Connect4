using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : AbstractManager
{
    #region Constants
    const string DrawMsg = "It's a Draw";
    const string WinMsg = "Player # Wins!";
    #endregion

    #region PrivateParams
    [SerializeField] Text _resultText;
    [SerializeField] GameObject _mainManu;
    [SerializeField] GameObject _manuButton;
    [SerializeField] GameObject _restartButton;
    [SerializeField] GameObject _pauseManu;
    [SerializeField] GameObject _settingsManu;
    [SerializeField] Slider musicVolSlider;
    [SerializeField] Slider sfxVolSlider;
    GameMode _modeChosen;
    #endregion

    #region ButtonHandlers
    public void OnRestartButtonClick()
    {
        AudioManager.Instance.PlaySound(SoundType.Click);
        StateMachine.SetNextState(GameState.RESTART);
        StateMachine.SetNextState(GameState.GAME);
    }

    public void OnExitPauseClick()
    {
        AudioManager.Instance.PlaySound(SoundType.Click);
        StateMachine.SetNextState(GameState.GAME);
    }
    public void OnPVPChosen()
    {
        AudioManager.Instance.PlaySound(SoundType.Click);
        _modeChosen = GameMode.PVP;
    }
    public void OnPVCChosen()
    {
        AudioManager.Instance.PlaySound(SoundType.Click);
        _modeChosen = GameMode.PVC;

    }
    public void OnCVCChosen()
    {
        AudioManager.Instance.PlaySound(SoundType.Click);
        _modeChosen = GameMode.CVC;
    }

    public void OnStartClick()
    {
        AudioManager.Instance.PlaySound(SoundType.Click);
        GameData.CurrentGameMode = _modeChosen;
        StateMachine.SetNextState(GameState.GAME);
    }




    public void OnOpenManuClick()
    {
        AudioManager.Instance.PlaySound(SoundType.Click);
        if (StateMachine.currentState == GameState.GAME)
        {
            StateMachine.SetNextState(GameState.PAUSE);

        }
        else
        {
            if (StateMachine.currentState == GameState.GAME_ENDED)
            {
                StateMachine.SetNextState(GameState.RESTART);
                StateMachine.SetNextState(GameState.MANU);
            }
        }
    }

    public void OnMainManuClick()
    {
        AudioManager.Instance.PlaySound(SoundType.Click);
        StateMachine.SetNextState(GameState.RESTART);
        StateMachine.SetNextState(GameState.MANU);
    }

    public void OnOpenSettingsClick()
    {
        AudioManager.Instance.PlaySound(SoundType.Click);
        StateMachine.SetNextState(GameState.SETTINGS);
    }

    public void OnCloseSettingsClick()
    {
        AudioManager.Instance.PlaySound(SoundType.Click);
        StateMachine.SetNextState(GameState.MANU);
    }
    #endregion

    #region ScriptLifeCycleFunctions

    protected override void ValidateParams()
    {
        if (!WinMsg.Contains("#"))
        {
            throw new Exception("Winning msg should contain # in order to show the player number");
        }
    }
    #endregion

    #region EndScreenFunctions
    private void OnGameEnded()
    {
        GameResults result = GameData.CurrentGameResults;
        if (result == GameResults.DRAW)
        {
            ShowMsg(DrawMsg);
        }
        else
        {
            ShowMsg(WinMsg.Replace("#",((int)result).ToString()));
        }
    }

    private void ShowMsg(string msg)
    {
        _resultText.text = msg;
        _resultText.gameObject.SetActive(true);
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
                _mainManu.SetActive(true);
                break;
            case GameState.GAME:
                _mainManu.SetActive(false);
                _restartButton.SetActive(true);
                _manuButton.SetActive(true);
                break;
            case GameState.GAME_ENDED:
                _restartButton.SetActive(true);
                _manuButton.SetActive(true);
                OnGameEnded();
                break;
            case GameState.PAUSE:
                _pauseManu.SetActive(true);
                _restartButton.SetActive(false);
                _manuButton.SetActive(false);
                break;
            case GameState.RESTART:
                break;
            case GameState.SETTINGS:
                _mainManu.SetActive(false);
                _settingsManu.SetActive(true);
                float musicVol, SFXVol;
                GameData.LoadPlayerMusicPrefs(out musicVol, out SFXVol);
                musicVolSlider.value = musicVol;
                sfxVolSlider.value = SFXVol;
                break;
            default:
                break;
        }
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
                _mainManu.SetActive(false);
                break;
            case GameState.GAME:
                break;
            case GameState.GAME_ENDED:
                _resultText.gameObject.SetActive(false);
                break;
            case GameState.PAUSE:
                _pauseManu.SetActive(false);
                break;
            case GameState.RESTART:
                break;
            case GameState.SETTINGS:
                GameData.SavePlayerMusicPrefs(musicVolSlider.value, sfxVolSlider.value);
                _settingsManu.SetActive(false);
                break;
            default:
                break;
        };
    }
    #endregion

    #region Setting
    public void OnMusicVolChange(float val)
    {
        AudioManager.Instance.SetMusicVol(val);

    }

    public void OnSFXVolChange(float val)
    {
        AudioManager.Instance.SetSFXVol(val);
    }

    public void OnDifficultChange(int val)
    {
        GameData.CurrentGameDifficulty = (Difficult) val;
    }
    #endregion
}
