using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopUI : MonoBehaviour
{
    public void BackToMainMenu()
    {
        GameManager.Instance.RestartGame();
        GameManager.Instance.IsPaused = false;
    }

    public void GoToNextLevel()
    {
        GameManager.Instance.GoToNextLevelFromShop();
    }
}
