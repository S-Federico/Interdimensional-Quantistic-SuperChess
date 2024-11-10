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
    public TMP_Text EarnedMoneyText;
    public TMP_Text CurrentMoneyText;
    public TMP_Text GameOverText;

    // public GameObject NextLevelBtn;
    public GameObject GoToShopBtn;
    public GameObject BackToMenuBtn;
    public Texture2D defaultCursor;
    public Texture2D handCursor;

    private static Texture2D _defaultCursor;
    private static Texture2D _handCursor;
    [SerializeField] private bool isTutorial = false;
    public bool IsTutorial { get => isTutorial; }
    void Start()
    {
        _defaultCursor = defaultCursor;
        _handCursor = handCursor;

        HideAll();

        // Play ost
        SoundManager.Instance.PlaySoud(Sound.OST_HEARTSONE, true, true, false);
    }

    void Update()
    {
        // Menu button has to be shown only if is not in GameOver
        MenuButton.SetActive(!GameManager.Instance.IsGameOver);

        // Pause Menu should be visible only if game is in pause and is not game over
        PauseMenu.SetActive(!GameManager.Instance.IsGameOver && GameManager.Instance.IsPaused);

        // GameOver UI should be visible only if is game over
        GameOverMenu.SetActive(GameManager.Instance.IsGameOver);

        if (GameManager.Instance.IsGameOver && !isTutorial)
        {
            GameInfo gameInfo = GameManager.Instance.GameInfo;
            if (gameInfo.Winner != null && gameInfo.Winner == PieceColor.White)
            {
                GameOverText.text = Constants.VICTORY_TEXT;
                EarnedMoneyText.text = $"Money won: {GameManager.Instance.MoneyWonFromCurrentRound}$";
                // NextLevelBtn.SetActive(true);
                GoToShopBtn.SetActive(true);
                BackToMenuBtn.SetActive(false);
            }
            else
            {
                GameOverText.text = Constants.DEFEAT_TEXT;
                EarnedMoneyText.text = "";
                BackToMenuBtn.SetActive(true);
                // NextLevelBtn.SetActive(false);
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
        SoundManager.Instance.PlaySoundOneShot(Sound.BUTTON_PRESSED);
    }

    public static void SetCursor(CursorType cursorType)
    {
        switch (cursorType)
        {
            case CursorType.Default:
                Cursor.SetCursor(_defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.Hand:
                Cursor.SetCursor(_handCursor, Vector2.zero, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(_defaultCursor, Vector2.zero, CursorMode.Auto);
                break;
        }
    }
}

