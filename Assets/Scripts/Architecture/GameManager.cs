using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class GameManager : Singleton<GameManager>
{

    // Percorso della cartella e del file di salvataggio (usa Application.dataPath per accedere alla cartella Assets)
    string SaveFolderPath;
    string SaveFilePath;

    public void Start()
    {
        SaveFolderPath = Application.dataPath + "/Files";
        SaveFilePath = SaveFolderPath + "/boardStatus.txt";
        // Crea la cartella di salvataggio se non esiste
        if (!Directory.Exists(SaveFolderPath))
        {
            Directory.CreateDirectory(SaveFolderPath);
        }
    }

    void Awake()
    {
        Debug.Log("Game Manager instantiated!");
    }

    public void NewGame()
    {
        Debug.Log("New Game pressed");
        // Pulisci il file esistente
        //File.WriteAllText(SaveFilePath, "");
        SceneManager.LoadScene("SampleScene");
    }

    public void ContinueGame()
    {
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
        if (boardStatus == null)
        {
            Debug.LogError("boardConfig è null!");
            return;
        }

        // Pulisci il file esistente
        File.WriteAllText(SaveFilePath, "");

        string json = JsonConvert.SerializeObject(boardStatus, Formatting.Indented); // Usa JSON.NET

        //Queste righe servono perché potremmo cambiare cose nel progetto e inserire nello stato 
        //da salvare oggetti non serializzabili. Questo serve a debuggare questa cosa, lo toglieremo
        //quando il modello dei dati non cambierà più
        BoardData b = JsonConvert.DeserializeObject<BoardData>(json);
        bool equal = b.Equals(boardStatus);
        Debug.Log("Abbiamo salvato bene? " + equal);

    }

    public void LoadGameFromFile()
    {
        string saveJson = System.IO.File.ReadAllText(SaveFilePath);
        BoardData b = JsonConvert.DeserializeObject<BoardData>(saveJson);

        var boardManager = GameObject.FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager non trovato!");
            return;
        }

        boardManager.BuildFromData(b);
    }

    public bool IsSaveFilePresent()
    {
        return File.Exists(SaveFilePath);
    }
}
