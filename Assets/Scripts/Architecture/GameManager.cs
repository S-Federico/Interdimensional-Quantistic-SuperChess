using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    void Awake() {
        Debug.Log("Game Manager instantiated!");
    }

    public void RestartGame() {
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

    public void SaveToFile(){
        var boardStatus = GameObject.FindAnyObjectByType<BoardManager>().SaveStatus();
        Debug.Log("HAI SALVATO YAY");
    }
}
