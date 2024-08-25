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


}
