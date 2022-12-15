using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Text _resultText;
    const string DrawMsg = "It's a Draw";
    const string WinMsg = "Player # Wins!"; 

    void ValidateParams()
    {
        if (!WinMsg.Contains("#"))
        {
            throw new Exception("Winning msg should contain # in order to show the player number");
        }
    }

    private void Start()
    {
        ValidateParams();
    }

    void OnEnable()
    {
        BoardManager.gameResultEvent += OnGameEnded;
    }

    void OnDisable()
    {
        BoardManager.gameResultEvent -= OnGameEnded;
    }

    private void OnGameEnded(GameResults result)
    {
        if(result == GameResults.DRAW)
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





}
