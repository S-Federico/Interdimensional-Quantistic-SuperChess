using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class BooleanOptionHandler : MonoBehaviour
{
    // Extern fields

    [SerializeField] private TextMeshProUGUI OptionNameText;
    public bool Enabled = true;
    public string Text;
    public bool Value;
    [SerializeField] private Toggle toggle;

    // Delegates
    public delegate void OnToggleValueChangeEventDelegate(bool newValue);
    public OnToggleValueChangeEventDelegate OnToggleValueChangeEvent;


    // Start is called before the first frame update
    void Start()
    {
        OptionNameText.text = Text;
        toggle.isOn = Value;
    }


    void OnEnable()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChange);
        toggle.onValueChanged.AddListener(PlayToggleSwitchSound);
    }

    void OnDisable()
    {
        toggle.onValueChanged.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        toggle.isOn = Value;
        toggle.interactable = Enabled;
    }

    private void OnToggleValueChange(bool newValue)
    {
        Value = newValue;
        OnToggleValueChangeEvent.Invoke(newValue);
    }

    private void PlayToggleSwitchSound(bool newToggleValue)
    {
        SoundManager.Instance.PlaySoundOneShot(Sound.BUTTON_PRESSED);
    }
}
