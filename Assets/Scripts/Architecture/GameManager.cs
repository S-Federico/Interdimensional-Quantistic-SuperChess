using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameManager : Singleton<GameManager>
{
    void Awake()
    {
        Debug.Log("Game Manager instantiated!");
    }

    public void RestartGame()
    {
        // When game restarts, load the main menu scene
        SceneManager.LoadScene("Menu");
    }

    //save to file
    //trova boardmanager
    //BoardStatus bs=boardmanager.getstaus
    //scrivi bs da qualche parte file

    //read
    //leggi file
    // crea un boardstatus
    //triva boardmanager e fai setstate(boardstatus)

    public void SaveToFile()
    {
        // Trova il BoardManager e controlla se esiste
        var boardManager = GameObject.FindAnyObjectByType<BoardManager>();
        if (boardManager == null)
        {
            Debug.LogError("BoardManager non trovato!");
            return;
        }

        // Chiama SaveStatus() e controlla se il risultato è valido
        BoardData boardStatus = boardManager.GetBoardData();
        if (boardStatus == null)
        {
            Debug.LogError("boardConfig è null!");
            return;
        }

        // Percorso della cartella e del file (usa Application.dataPath per accedere alla cartella Assets)
        string folderPath = Application.dataPath + "/Files";
        string filePath = folderPath + "/boardStatus.txt";

        // Crea la cartella se non esiste
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Pulisci il file esistente
        File.WriteAllText(filePath, "");

        string json = JsonConvert.SerializeObject(boardStatus, Formatting.Indented); // Usa JSON.NET
        Debug.Log(json);

        BoardData b = JsonConvert.DeserializeObject<BoardData>(json);

        // Confronto delle matrici elemento per elemento
        bool equal = b.Equals(boardStatus);
        Debug.Log("Abbiamo salvato bene? " + equal);

        //Debug.Log("HAI SALVATO YAY (" + filePath + ")");
    }

}
