using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    public Button StartGameButton;
    public Button OptionsButton;
    // Start is called before the first frame update
    void Start()
    {
        // Start Game button events
        StartGameButton.onClick.AddListener(StartGamePressed);
        StartGameButton.onClick.AddListener(PlayButtonSound);

        // Options button events
        OptionsButton.onClick.AddListener(OptionsPressed);
        OptionsButton.onClick.AddListener(PlayButtonSound);
    }

    void OnDestroy()
    {
        // Start Game button events
        StartGameButton.onClick.RemoveListener(StartGamePressed);
        StartGameButton.onClick.RemoveListener(PlayButtonSound);

        // Options button events
        OptionsButton.onClick.RemoveListener(OptionsPressed);
        OptionsButton.onClick.RemoveListener(PlayButtonSound);
    }

    private void StartGamePressed()
    {
        //SceneManager.LoadScene(Constants.Scenes.MENU);
        GameManager.Instance.RestartGame();
    }

    private void OptionsPressed()
    {
        //SceneManager.LoadScene(Constants.Scenes.OPTIONS);
        GameManager.Instance.LoadScene(Constants.Scenes.OPTIONS);
    }

    private void PlayButtonSound()
    {
        SoundManager.PlaySoundOneShot(Sound.BUTTON_PRESSED);
    }
}
