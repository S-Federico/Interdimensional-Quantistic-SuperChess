using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    // Elements
    [SerializeField] private GameObject ImagePanel;
    [SerializeField] private TextMeshProUGUI TextElement;
    [SerializeField] private Button NextButton;
    [SerializeField] private Button FinishButton;

    // Dialogue Lines
    public List<string> DialogueLines = new List<string>();

    // What line is currently displaying
    private int currentLineIndex = 0;
    public int CurrentLineIndex { get => currentLineIndex; }

    // Sprite to insert in dialogue
    public Sprite TalkerSprite;

    public delegate void OnNewLine(int newIndex);
    public OnNewLine OnNewLineHandler;
    public delegate void OnFinishDialogue();
    public OnFinishDialogue OnFinishDialogueHandler;

    // Start is called before the first frame update
    void Awake()
    {
        ImagePanel.GetComponent<Image>().sprite = TalkerSprite;
    }

    // Update is called once per frame
    void Update()
    {
         // Check which buttons display
        if (currentLineIndex < DialogueLines.Count - 1) {
            NextButton.interactable = true;
            FinishButton.interactable = false;
        } else {
            NextButton.interactable = false;
            FinishButton.interactable = true;
        }

        // Check witch text line display
        if (currentLineIndex >= 0 && currentLineIndex < DialogueLines.Count) {
            TextElement.text = DialogueLines[currentLineIndex];
        }
    }

    void OnEnable()
    {
        NextButton.onClick.AddListener(OnNextClick);
        NextButton.onClick.AddListener(PlayButtonSound);

        FinishButton.onClick.AddListener(OnFinishClick);
        FinishButton.onClick.AddListener(PlayButtonSound);
    }

    void OnDisable()
    {
        NextButton.onClick.RemoveAllListeners();
        FinishButton.onClick.RemoveAllListeners();
    }

    private void OnNextClick()
    {
        currentLineIndex++;
        OnNewLineHandler?.Invoke(currentLineIndex);
    }

    private void OnFinishClick()
    {
        OnFinishDialogueHandler?.Invoke();
    }

    private void PlayButtonSound()
    {
        SoundManager.Instance.PlaySoundOneShot(Sound.BUTTON_PRESSED);
    }
}
