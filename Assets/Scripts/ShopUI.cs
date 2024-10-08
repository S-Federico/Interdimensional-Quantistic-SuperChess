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
        #warning Controllare cosa si deve salvare in questo caso. Adesso non trova BuardManager (giustamente)
        GameManager.Instance.SaveGameToFile();
        GameManager.Instance.RestartGame();
        GameManager.Instance.IsPaused = false;
    }

    public void GoToNextLevel()
    {
        #warning Controllare cosa si deve salvare in questo caso. Adesso non trova BuardManager (giustamente)
        GameManager.Instance.AdvanceLevel();
        GameInfo gameInfo = GameManager.Instance.GameInfo;
        List<PieceData> enemies = LevelGenerator.Instance.GeneratePieces("Pieces","Modifiers",PieceColor.Black,gameInfo.currentLevel, gameInfo.currentStage);
        gameInfo.OpponentInfo.ExtraPieces = enemies;
        gameInfo.BoardData = LevelGenerator.Instance.GenerateDefaultBoardData();
        GameManager.Instance.IsGameOver = false;
        GameManager.Instance.SaveGameToFile();
        GameManager.Instance.ContinueGame(gameInfo);
    }
}
