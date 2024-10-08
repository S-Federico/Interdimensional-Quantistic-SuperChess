using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject GameOverMenu;
    public GameObject MenuButton;
    public TMP_Text TotalMoneyText;
    public TMP_Text EarnedMoneyText;

    void Start()
    {
        HideAll();
    }
    
    void Update() {
        // Menu button has to be shown only if is not in GameOver
        MenuButton.SetActive(!GameManager.Instance.IsGameOver);

        // Pause Menu should be visible only if game is in pause and is not game over
        PauseMenu.SetActive(!GameManager.Instance.IsGameOver && GameManager.Instance.IsPaused);

        // GameOver UI should be visible only if is game over
        GameOverMenu.SetActive(GameManager.Instance.IsGameOver);

        if (GameManager.Instance.IsGameOver) {
            EarnedMoneyText.text = $"Money won: {GameManager.Instance.MoneyWonFromCurrentRound}$";
            TotalMoneyText.text = $"Total money: {GameManager.Instance.GameInfo.PlayerInfo.Money}$";
        }
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

    public void MenuButtonPressed() {
        GameManager.Instance.IsPaused = !GameManager.Instance.IsPaused;
    }

    public void GoToNextLevel() {
        GameManager.Instance.AdvanceLevel();
        GameInfo gameInfo = GameManager.Instance.GameInfo;
        List<PieceData> enemies = LevelGenerator.Instance.GenerateEnemies(gameInfo.currentLevel, gameInfo.currentStage);
        gameInfo.OpponentInfo.ExtraPieces = enemies;
        gameInfo.BoardData = LevelGenerator.Instance.GenerateDefaultBoardData();
        GameManager.Instance.IsGameOver = false;
        GameManager.Instance.SaveGameToFile();
        GameManager.Instance.ContinueGame(gameInfo);
    }

    public void GoToShop() {
        SceneManager.LoadScene("ShopMockup");
    }
}
