using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Options
{
    public const float MAX_VOLUME_VALUE = 100f;
    public const float MIN_VOLUME_VALUE = 0f;

    // Music related
    public bool MusicEnabled = true;
    private float musicVolume = 50f;

    public float MusicVolumeClamped { get => musicVolume / (MAX_VOLUME_VALUE - MIN_VOLUME_VALUE); }

    public float MusicVolume
    {
        get => musicVolume;
        set => musicVolume = Mathf.Clamp(value, MIN_VOLUME_VALUE, MAX_VOLUME_VALUE);
    }

    // Sound effect related
    public bool SoundEnabled = true;
    private float soundVolume = 50f;
    public float SoundVolumeClamped { get => soundVolume / (MAX_VOLUME_VALUE - MIN_VOLUME_VALUE); }


    public float SoundVolume
    {
        get => soundVolume;
        set => soundVolume = Mathf.Clamp(value, MIN_VOLUME_VALUE, MAX_VOLUME_VALUE);
    }

    public void SetMusicEnabled(bool value)
    {
        MusicEnabled = value;
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
    }

    public void SetSoundEnabled(bool value)
    {
        SoundEnabled = value;
    }

    public void SetSoundVolume(float value)
    {
        SoundVolume = value;
    }

    internal void Reset()
    {
        MusicEnabled = true;
        SoundEnabled = true;
        MusicVolume = 50f;
        SoundVolume = 50f;
    }
}
