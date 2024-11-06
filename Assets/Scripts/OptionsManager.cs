using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{

    public GameObject optionComponentsPanel;

    // Start is called before the first frame update
    void Start()
    {
        AddBooleanComponent("Music enabled", GameManager.Instance.Options.SetMusicEnabled, GameManager.Instance.Options.MusicEnabled);
        AddBooleanComponent("Sound enabled", GameManager.Instance.Options.SetSoundEnabled, GameManager.Instance.Options.SoundEnabled);
    }

    void OnDestroy() {
        foreach (Transform child in optionComponentsPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddBooleanComponent(string title, BooleanOptionHandler.OnToggleValueChangeEventDelegate callback, bool startValue) {
        GameObject newElement = Instantiate(AssetsManager.Instance.BooleanOptionPrefab, optionComponentsPanel.transform);
        BooleanOptionHandler booleanOptionHandler = newElement.GetComponent<BooleanOptionHandler>();
        booleanOptionHandler.Enabled = true;
        booleanOptionHandler.Text = title;
        booleanOptionHandler.Value = startValue;
        booleanOptionHandler.OnToggleValueChangeEvent += callback;
    }
}
