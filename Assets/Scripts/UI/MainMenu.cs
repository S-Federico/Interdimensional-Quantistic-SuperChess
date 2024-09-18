using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Inspector references
    public GameObject continueButton;
    public GameObject LoadGamePanel;
    public Transform ProfilesContainer;
    public GameObject ProfileItemPrefab;
    public GameObject MainMenuPanel;
    public GameObject ChooseProfileNamePanel;
    public TMP_InputField ProfileNameInput;
    public int MaxSaves = 3;

    // Private variables
    private GameManager gameManager;
    private GameInfo[] savedGames;

    private GameInfo selectedGameInfo;

    void Start()
    {
        savedGames = new GameInfo[MaxSaves];
        gameManager = GameManager.Instance;
        Reset();
        string[] saveFiles = SaveManager.Instance.FindSaveFiles();
        int i = 0;
        if (saveFiles != null && saveFiles.Length > 0)
        {
            for (i = 0; i < saveFiles.Length && i < MaxSaves; i++)
            {
                GameInfo gameInfo = SaveManager.Instance.Load<GameInfo>(saveFiles[i], true);
                Debug.Log($"Is gameInfo != null? {gameInfo != null}");
                savedGames[i] = gameInfo;
                GameObject profileItem = Instantiate(this.ProfileItemPrefab, ProfilesContainer);
                profileItem.GetComponentInChildren<TextMeshProUGUI>().text = gameInfo.ProfileName;

                // Add a click listener to the button to handle click events
                Button buttonComponent = profileItem.GetComponent<Button>();
                buttonComponent.onClick.AddListener(() => OnFileButtonClick(gameInfo));
            }
        }
        if (i < MaxSaves)
        {
            for (; i < MaxSaves; i++)
            {
                GameInfo gameInfo = new GameInfo();
                savedGames[i] = gameInfo;
                GameObject profileItem = Instantiate(this.ProfileItemPrefab, ProfilesContainer);
                profileItem.GetComponentInChildren<TextMeshProUGUI>().text = "Empty";

                // Add a click listener to the button to handle click events
                Button buttonComponent = profileItem.GetComponent<Button>();
                buttonComponent.onClick.AddListener(() => OnFileButtonClick(gameInfo));
            }
        }

    }

    private void OnFileButtonClick(GameInfo gameInfo)
    {
        this.selectedGameInfo = gameInfo;
        if (!gameInfo.HasSaveFile())
        {
            // So it's an empty profile
            CreateNewProfilePressed();
        }
        else
        {
            LoadMenuPanel();
        }
    }

    public void NewGame()
    {
        GameManager.Instance.NewGame(this.selectedGameInfo);
    }

    public void Reset()
    {
        LoadGamePanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        ChooseProfileNamePanel.SetActive(false);
    }

    public void LoadMenuPanel()
    {
        LoadGamePanel.SetActive(false);
        MainMenuPanel.SetActive(true);
        ChooseProfileNamePanel.SetActive(false);
    }

    public void CreateNewProfilePressed()
    {
        LoadGamePanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        ChooseProfileNamePanel.SetActive(true);
    }

    public void CreatenewProfileConfirmed()
    {
        this.selectedGameInfo.ProfileName = ProfileNameInput.text;
        LoadMenuPanel();
        this.ProfileNameInput.text = null;
    }

    public void LoadGamePressed() {
        GameManager.Instance.ContinueGame(this.selectedGameInfo);
    }

    public void DeleteProfilePressed() {
        PopupManager.Instance.ShowPopup("Are you sure?", () => {
            //TODO: Delete file from selectedGameInfo
            SaveManager.Instance.DeleteFile(this.selectedGameInfo.ProfileName);
            this.selectedGameInfo = null;
            GameManager.Instance.RestartGame();
        }, () => {});
    }
}
