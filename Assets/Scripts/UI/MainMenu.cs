using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject continueButton;
    public GameObject LoadGamePanel;
    public GameObject MainMenuPanel;
    void Start(){
        GameManager gm = GameManager.Instance;
        if(gm.IsSaveFilePresent()){
            continueButton.SetActive(true);
        }
        Reset();
    }
    public void NewGame() {
        GameManager.Instance.NewGame();
    }
    public void ContinueGame(){
        //GameManager.Instance.ContinueGame();
        LoadGamePanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }

    public void Reset() {
        LoadGamePanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }
}
