using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class SliderOptionHandler : MonoBehaviour
{
    // Extern fields

    [SerializeField] private TextMeshProUGUI OptionNameText;
    public bool Enabled = true;
    public string Text;
    public float Value;
    public int MinValue = 0;
    public int MaxValue = 100;
    public bool intStepMode = true;
    [SerializeField] private Slider Slider;
    [SerializeField] private TextMeshProUGUI SliderValueText;

    // Delegates
    public delegate void OnSliderValueChangeEventDelegate(float newValue);
    public OnSliderValueChangeEventDelegate OnSliderValueChangeEvent;


    // Start is called before the first frame update
    void Start()
    {
        OptionNameText.text = Text;
        Slider.value = GetValueWithMode(Value, intStepMode);
    }


    void OnEnable()
    {
        Slider.onValueChanged.AddListener(OnSliderValueChange);
    }

    void OnDisable()
    {
        Slider.onValueChanged.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        Slider.value = GetValueWithMode(Value, intStepMode);
        Slider.interactable = Enabled;
        SliderValueText.text = "" + GetValueWithMode(Value, intStepMode);
    }

    private void OnSliderValueChange(float newValue)
    {
        Value = GetValueWithMode(newValue, intStepMode);
        OnSliderValueChangeEvent.Invoke(Value);
    }

    // private void PlayToggleSwitchSound(bool newToggleValue)
    // {
    //     SoundManager.PlaySoundOneShot(Sound.BUTTON_PRESSED);
    // }

    private float GetValueWithMode(float value, bool intStepMode) {
        return intStepMode ? ((int)value) : value;
    }
}
