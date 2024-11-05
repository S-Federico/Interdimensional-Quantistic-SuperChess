using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Options
{
    // Music related
    public bool MusicEnabled = true;
    private float musicVolume = 1f;

    public float MusicVolume {
        get => musicVolume;
        set => musicVolume = Mathf.Clamp(value, 0f, 1f);
    }

    // Sound effect related
    public bool SoundEnabled = true;
    private float soundVolume = 1f;

    public float SoundVolume {
        get => soundVolume;
        set => soundVolume = Mathf.Clamp(value, 0f, 1f);
    }
}
