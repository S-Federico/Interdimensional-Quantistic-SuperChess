using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System;
using System.Linq;

public class GameManager : Singleton<GameManager>
{
    public static int MAX_LEVEL = 3;


    public GameInfo GameInfo;
    private bool isPaused = false;
    private bool isGameOver = false;

    public bool IsGameOver { get => isGameOver; set => isGameOver = value; }
    public bool IsPaused { get => isPaused; set => isPaused = value; }

    void Awake()
    {
        Debug.Log("Game Manager instantiated!");
    }

    public IEnumerator NewGameCoroutine(string sceneName)
    {
        // Carica la scena in modo asincrono
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Aspetta fino a quando la scena non è completamente caricata
        while (!asyncLoad.isDone)
        {
            yield return null; // Aspetta un frame
        }

        var boardManager = GameObject.FindAnyObjectByType<BoardManager>();
        boardManager.LoadBoardFromBoardData();
        boardManager.InitializePiecesPlanes(true);
    }

    public void NewGame(GameInfo gameInfo)
    {
        this.GameInfo = gameInfo;
        StartCoroutine(NewGameCoroutine("SampleScene"));
    }

    public void ContinueGame(GameInfo gameInfo)
    {
        this.GameInfo = gameInfo;
        StartCoroutine(LoadSceneAndContinue("SampleScene"));
    }

    private IEnumerator LoadSceneAndContinue(string sceneName)
    {
        // Carica la scena in modo asincrono
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Aspetta fino a quando la scena non è completamente caricata
        while (!asyncLoad.isDone)
        {
            yield return null; // Aspetta un frame
        }

        // La scena è caricata completamente, ora puoi chiamare il metodo per caricare i dati di gioco
        LoadGameFromFile();
    }

    public void RestartGame()
    {
        // When game restarts, load the main menu scene
        SceneManager.LoadScene("Menu");
        isGameOver = false;
    }


    public void SaveGameToFile()
    {
        var boardManager = GameObject.FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager non trovato!");
            return;
        }

        // Chiama GetBoardData() e controlla se il risultato è valido
        BoardData boardStatus = boardManager.GetBoardData();
        this.GameInfo.BoardData = boardStatus;
        if (boardStatus == null)
        {
            Debug.LogError("boardConfig è null!");
            return;
        }

        // Salvo i pezzi usati attualmente dall'opponent nel file di salvataggio
        OpponentManager opponentManager = FindAnyObjectByType<OpponentManager>();
        if (opponentManager != null) {
            GameInfo.OpponentInfo.CurrentlyUsedExtraPieces = new List<PieceData>();
            opponentManager.pieces.ForEach(p => GameInfo.OpponentInfo.CurrentlyUsedExtraPieces.Add(p.GetPieceData()));
        }

        // Salvo i pezzi usati attualmente dal player nel file di salvataggio
        PlayerManager playerManager = FindAnyObjectByType<PlayerManager>();
        if (playerManager != null) {
            GameInfo.PlayerInfo.CurrentlyUsedExtraPieces = new List<PieceData>();
            playerManager.pieces.ForEach(p => GameInfo.PlayerInfo.CurrentlyUsedExtraPieces.Add(p.GetPieceData()));
        }


        // string json = JsonConvert.SerializeObject(boardStatus, Formatting.Indented); // Usa JSON.NET
        SaveManager.Instance.Save(GameInfo, this.GameInfo.ProfileName);

        // //Queste righe servono perché potremmo cambiare cose nel progetto e inserire nello stato 
        // //da salvare oggetti non serializzabili. Questo serve a debuggare questa cosa, lo toglieremo
        // //quando il modello dei dati non cambierà più
        // BoardData b = JsonConvert.DeserializeObject<BoardData>(json);
        // bool equal = b.Equals(boardStatus);
        // Debug.Log("Abbiamo salvato bene? " + equal);

    }

    public void LoadGameFromFile()
    {
        //GameInfo b = SaveManager.Instance.Load<GameInfo>(gameInfo.ProfileName);

        var boardManager = GameObject.FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager non trovato!");
            return;
        }

        boardManager.BuildFromData(this.GameInfo.BoardData);
    }

    internal void GameOver()
    {
        this.isGameOver = true;

        // Give money
        this.GameInfo.PlayerInfo.Money += MoneyWonFromCurrentRound;

        SaveGameToFile();
    }

    public int MoneyWonFromCurrentRound {
        get => 2 * GameInfo.currentLevel * GameInfo.currentStage;
    }

    internal void AdvanceLevel()
    {
        GameInfo.currentStage++;
        if (GameInfo.currentStage == MAX_LEVEL + 1)
        {
            GameInfo.currentLevel += 1;
            GameInfo.currentStage = 1;
        }
    }
}
