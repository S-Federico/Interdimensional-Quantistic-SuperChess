using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameObject gameMenu;

    void Start()
    {
        HideGameMenu();
    }

    public void ShowGameMenu()
    {
        gameMenu.SetActive(true);
    }
    public void HideGameMenu()
    {
        gameMenu.SetActive(false);
    }
    public void BackToMainMenu()
    {
        GameManager.Instance.RestartGame();
    }
}
