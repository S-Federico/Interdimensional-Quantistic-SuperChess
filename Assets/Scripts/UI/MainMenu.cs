using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public Button LoadGamePanelBackButton;

    // Private variables
    private GameManager gameManager;
    private GameInfo[] savedGames;

    private GameInfo selectedGameInfo;
    private Button continueGameButton;

    void Start()
    {
        continueGameButton = continueButton.GetComponent<Button>();
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

        SoundManager.PlaySoud(Sound.OST, true, true);

        LoadGamePanelBackButton.onClick.AddListener(OnGamePanelBackButtonPressed);
        LoadGamePanelBackButton.onClick.AddListener(PlayButtonSound);

    }

    private void OnFileButtonClick(GameInfo gameInfo)
    {
        PlayButtonSound();
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
        PlayButtonSound();
        this.selectedGameInfo.Reset();
        this.selectedGameInfo.GameStarted = true;
        GameManager.Instance.NewGame(this.selectedGameInfo);
    }

    public void BackButtonPressed()
    {
        PlayButtonSound();
        Reset();
    }

    public void Reset()
    {
        LoadGamePanel.SetActive(true);
        MainMenuPanel.SetActive(false);
        ChooseProfileNamePanel.SetActive(false);
    }

    public void LoadMenuPanel()
    {
        PlayButtonSound();
        LoadGamePanel.SetActive(false);
        MainMenuPanel.SetActive(true);
        ChooseProfileNamePanel.SetActive(false);
    }

    public void CreateNewProfilePressed()
    {
        PlayButtonSound();
        LoadGamePanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        ChooseProfileNamePanel.SetActive(true);
    }

    public void CreatenewProfileConfirmed()
    {
        PlayButtonSound();
        this.selectedGameInfo.ProfileName = ProfileNameInput.text;
        LoadMenuPanel();
        this.ProfileNameInput.text = null;
    }

    public void LoadGamePressed()
    {
        PlayButtonSound();
        GameManager.Instance.ContinueGame(this.selectedGameInfo);
    }

    public void DeleteProfilePressed()
    {
        PlayButtonSound();
        PopupManager.Instance.ShowPopup("Are you sure?", () =>
        {
            PlayButtonSound();
            //TODO: Delete file from selectedGameInfo
            SaveManager.Instance.DeleteFile(this.selectedGameInfo.ProfileName);
            this.selectedGameInfo = null;
            GameManager.Instance.RestartGame();
        }, () =>
        {
            PlayButtonSound();
        });
    }

    void Update()
    {
        continueGameButton.interactable = this.selectedGameInfo != null && this.selectedGameInfo.GameStarted;
    }

    private void PlayButtonSound()
    {
        SoundManager.PlaySoundOneShot(Sound.BUTTON_PRESSED);
    }

    private void OnGamePanelBackButtonPressed()
    {
        SceneManager.LoadScene(Constants.Scenes.START_SCREEN);
    }

    void OnDestroy()
    {
        LoadGamePanelBackButton.onClick.RemoveListener(OnGamePanelBackButtonPressed);
        LoadGamePanelBackButton.onClick.RemoveListener(PlayButtonSound);
    }
}
