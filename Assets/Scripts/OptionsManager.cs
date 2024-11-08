using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{

    // Exter references
    public GameObject optionComponentsPanel;
    public Button backButton;
    public Button saveChangesButton;
    public Button resetToDefaultsButton;

    // Private fields
    private BooleanOptionHandler musicEnabledHandler;
    private BooleanOptionHandler soundEnabledHandler;
    private SliderOptionHandler soundVolumeHandler;
    private SliderOptionHandler musicVolumeHandler;
    private Options options;

    // Start is called before the first frame update
    void Start()
    {
        options = GameManager.Instance.Options;
        musicEnabledHandler = AddBooleanComponent("Music enabled", GameManager.Instance.Options.SetMusicEnabled, GameManager.Instance.Options.MusicEnabled);
        musicVolumeHandler = AddSliderComponent("Music volume", GameManager.Instance.Options.SetMusicVolume, GameManager.Instance.Options.MusicVolume);
        soundEnabledHandler = AddBooleanComponent("Sound enabled", GameManager.Instance.Options.SetSoundEnabled, GameManager.Instance.Options.SoundEnabled);
        soundVolumeHandler = AddSliderComponent("Sound volume", GameManager.Instance.Options.SetSoundVolume, GameManager.Instance.Options.SoundVolume);

    }

    void OnDestroy()
    {
        foreach (Transform child in optionComponentsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void OnEnable()
    {
        backButton.onClick.AddListener(ReturnToPreviousScene);
        backButton.onClick.AddListener(PlayButtonSound);

        saveChangesButton.onClick.AddListener(SaveChanges);
        saveChangesButton.onClick.AddListener(PlayButtonSound);

        resetToDefaultsButton.onClick.AddListener(ResetToDefaults);
        resetToDefaultsButton.onClick.AddListener(PlayButtonSound);
    }



    void OnDisable()
    {
        backButton.onClick.RemoveAllListeners();
        saveChangesButton.onClick.RemoveAllListeners();
        resetToDefaultsButton.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        musicEnabledHandler.Value = options.MusicEnabled;
        soundEnabledHandler.Value = options.SoundEnabled;
        musicVolumeHandler.Value = options.MusicVolume;
        soundVolumeHandler.Value = options.SoundVolume;
    }

    private BooleanOptionHandler AddBooleanComponent(string title, BooleanOptionHandler.OnToggleValueChangeEventDelegate callback, bool startValue)
    {
        GameObject newElement = Instantiate(AssetsManager.Instance.BooleanOptionPrefab, optionComponentsPanel.transform);
        BooleanOptionHandler booleanOptionHandler = newElement.GetComponent<BooleanOptionHandler>();
        booleanOptionHandler.Enabled = true;
        booleanOptionHandler.Text = title;
        booleanOptionHandler.Value = startValue;
        booleanOptionHandler.OnToggleValueChangeEvent += callback;
        return booleanOptionHandler;
    }

    private SliderOptionHandler AddSliderComponent(string title, SliderOptionHandler.OnSliderValueChangeEventDelegate callback, float startValue)
    {
        GameObject newElement = Instantiate(AssetsManager.Instance.SliderOptionPrefab, optionComponentsPanel.transform);
        SliderOptionHandler booleanOptionHandler = newElement.GetComponent<SliderOptionHandler>();
        booleanOptionHandler.Enabled = true;
        booleanOptionHandler.Text = title;
        booleanOptionHandler.Value = startValue;
        booleanOptionHandler.OnSliderValueChangeEvent += callback;
        booleanOptionHandler.intStepMode = true;
        booleanOptionHandler.MinValue = 0;
        booleanOptionHandler.MaxValue = 100;
        return booleanOptionHandler;
    }


    // Button actions
    private void SaveChanges()
    {
        GameManager.Instance.SaveOptions();
    }

    private void ReturnToPreviousScene()
    {
        SceneManager.LoadScene(Constants.Scenes.START_SCREEN);
    }

    private void PlayButtonSound()
    {
        SoundManager.Instance.PlaySoundOneShot(Sound.BUTTON_PRESSED);
    }
    private void ResetToDefaults()
    {
        GameManager.Instance.Options.Reset();
    }
}
