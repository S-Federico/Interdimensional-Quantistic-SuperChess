using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopUI : MonoBehaviour
{
    void Start()
    {
    }

    void Update()
    {
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.RestartGame();
        GameManager.Instance.IsPaused = false;
    }

    public void BackToGame()
    {
       GameManager.Instance.ContinueGame(GameManager.Instance.GameInfo);
    }
}
