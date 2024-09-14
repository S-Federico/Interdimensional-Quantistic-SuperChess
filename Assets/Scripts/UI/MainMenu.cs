using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject continueButton;
    void Start(){
        GameManager gm = GameManager.Instance;
        if(gm.IsSaveFilePresent()){
            continueButton.SetActive(true);
        }
    }
    public void NewGame() {
        GameManager.Instance.NewGame();
    }
    public void ContinueGame(){
        GameManager.Instance.ContinueGame();
    }
}
