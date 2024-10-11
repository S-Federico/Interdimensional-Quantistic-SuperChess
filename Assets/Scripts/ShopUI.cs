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
#warning Controllare cosa si deve salvare in questo caso. Va cercando BoardManager(?)
        GameManager.Instance.SaveGameToFile();
        GameManager.Instance.RestartGame();
        GameManager.Instance.IsPaused = false;
    }

    public void GoToNextLevel()
    {
       GameManager.Instance.GoToNextLevel();
    }
}
