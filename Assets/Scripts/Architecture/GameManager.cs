using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System;

public class GameManager : Singleton<GameManager>
{
    private GameInfo gameInfo;
    public void Start()
    {

    }

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
    }

    public void NewGame(GameInfo gameInfo)
    {
        this.gameInfo = gameInfo;
        StartCoroutine(NewGameCoroutine("SampleScene"));
    }

    public void ContinueGame(GameInfo gameInfo)
    {
        this.gameInfo = gameInfo;
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
        this.gameInfo.BoardData = boardStatus;
        if (boardStatus == null)
        {
            Debug.LogError("boardConfig è null!");
            return;
        }

        // // Pulisci il file esistente
        // File.WriteAllText(SaveFilePath, "");

        // string json = JsonConvert.SerializeObject(boardStatus, Formatting.Indented); // Usa JSON.NET
        SaveManager.Instance.Save(gameInfo, this.gameInfo.ProfileName);

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

        boardManager.BuildFromData(this.gameInfo.BoardData);
    }
}
