using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        var boardStatus = boardManager.SaveStatus();
        if (boardStatus.boardConfig == null)
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

        // Ottieni le dimensioni dell'array bidimensionale
        int rows = boardStatus.boardConfig.GetLength(0);
        int cols = boardStatus.boardConfig.GetLength(1);

        // Itera su ogni elemento dell'array bidimensionale
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                PieceStatus piece = boardStatus.boardConfig[i, j];
                if (piece != null)
                { // Controlla se piece è null
                    string pieceS = piece.Code;

                    // Salva in append il contenuto di pieceS nel file
                    File.AppendAllText(filePath, pieceS + "\n"); // Aggiunge una nuova riga per ogni pieceS
                }
            }
        }

        Debug.Log("HAI SALVATO YAY (" + filePath + ")");
    }

}
