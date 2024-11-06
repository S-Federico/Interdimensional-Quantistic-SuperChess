using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject GameOverMenu;
    public GameObject MenuButton;
    public TMP_Text TotalMoneyText;
    public TMP_Text EarnedMoneyText;
    public TMP_Text CurrentMoneyText;
    public TMP_Text GameOverText;

    public GameObject NextLevelBtn;
    public GameObject GoToShopBtn;
    public GameObject BackToMenuBtn;

    void Start()
    {
        HideAll();
    }

    void Update()
    {
        // Menu button has to be shown only if is not in GameOver
        MenuButton.SetActive(!GameManager.Instance.IsGameOver);

        // Pause Menu should be visible only if game is in pause and is not game over
        PauseMenu.SetActive(!GameManager.Instance.IsGameOver && GameManager.Instance.IsPaused);

        // GameOver UI should be visible only if is game over
        GameOverMenu.SetActive(GameManager.Instance.IsGameOver);

        if (GameManager.Instance.IsGameOver)
        {
            GameInfo gameInfo = GameManager.Instance.GameInfo;
            if (gameInfo.Winner != null && gameInfo.Winner == PieceColor.White)
            {
                GameOverText.text = Constants.VICTORY_TEXT;
                EarnedMoneyText.text = $"Money won: {GameManager.Instance.MoneyWonFromCurrentRound}$";
                TotalMoneyText.text = $"Total money: {GameManager.Instance.GameInfo.PlayerInfo.Money}$";
                NextLevelBtn.SetActive(true);
                GoToShopBtn.SetActive(true);
                BackToMenuBtn.SetActive(false);
            }
            else
            {
                GameOverText.text = Constants.DEFEAT_TEXT;
                EarnedMoneyText.text = "";
                TotalMoneyText.text = "";
                BackToMenuBtn.SetActive(true);
                NextLevelBtn.SetActive(false);
                GoToShopBtn.SetActive(false);
            }
        }
        CurrentMoneyText.text = $"Total money: {GameManager.Instance.GameInfo.PlayerInfo.Money}$";
    }

    public void HideAll()
    {
        PauseMenu.SetActive(false);
        GameOverMenu.SetActive(false);
    }

    public void ShowPauseMenu()
    {
        PauseMenu.SetActive(true);
        GameOverMenu.SetActive(false);
    }
    public void ShowGameOverMenu()
    {
        PauseMenu.SetActive(false);
        GameOverMenu.SetActive(true);
    }
    public void BackToMainMenu()
    {
        GameManager.Instance.SaveGameToFile();
        GameManager.Instance.RestartGame();
        GameManager.Instance.IsPaused = false;
    }

    public void MenuButtonPressed()
    {
        GameManager.Instance.IsPaused = !GameManager.Instance.IsPaused;
    }

    public void GoToNextLevel()
    {
        GameManager.Instance.GoToNextLevel();
    }

    public void GoToShop()
    {
        //SceneManager.LoadScene("ShopMockup");
        GameManager.Instance.LoadScene(Constants.Scenes.SHOP);
    }

    private void PlayButtonSound()
    {
        SoundManager.PlaySoundOneShot(Sound.BUTTON_PRESSED);
    }
}
