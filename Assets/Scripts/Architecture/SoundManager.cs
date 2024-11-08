using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private Options options;

    private Dictionary<Sound, GameObject> NonOneShotSounds = new Dictionary<Sound, GameObject>();

    void Awake() {
        options = GameManager.Instance.Options;
    }

    void Update() {

        if (NonOneShotSounds.Keys.Count == 0) return;

        if (!options.MusicEnabled) {
            StopAllSounds();
        }

        foreach (var item in NonOneShotSounds.Keys)
        {
            AudioSource audioSource = NonOneShotSounds[item]?.GetComponent<AudioSource>();
            if (audioSource != null) {
                audioSource.volume = GameManager.Instance.Options.MusicVolume;
            }
        }
    }

    public void PlaySoundOneShot(Sound sound)
    {
        if (!options.SoundEnabled) return;

        GameObject gameObject = new GameObject("SOUND");
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        AudioClip audioClip = GetSoundInfo(sound)?.AudioClip;
        audioSource.volume = options.SoundVolumeClamped;
        audioSource.PlayOneShot(audioClip);

        // Prevent object destruction through scenes
        GameObject.DontDestroyOnLoad(gameObject);

        // Destroy the temporary sound gameObject after it finishes playig
        GameObject.Destroy(gameObject, audioClip.length);
    }

    /// <summary>
    /// This function plays a sound non one-shot. So the gameobject keep being persisted
    /// </summary>
    /// <param name="sound">The sound to play</param>
    /// <param name="looping">Looping mode on/off</param>
    /// <param name="alone">If this sound should pause all other sounds playing</param>
    /// /// <param name="forceRestart">If true, the song will restart also if it was already playing</param>
    public void PlaySoud(Sound sound, bool looping = true, bool alone = false, bool forceRestart = false)
    {
        if (!options.SoundEnabled) return;

        // If not forceRestart, then check if there is the sound already playing
        if (!forceRestart && NonOneShotSounds.ContainsKey(sound))
        {
            GameObject soundObj = NonOneShotSounds[sound];
            if (soundObj != null && soundObj.GetComponent<AudioSource>().isPlaying)
            {
                return;
            }
        }

        GameObject gameObject = new GameObject($"Sound_{sound}");
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        AudioClip audioClip = GetSoundInfo(sound)?.AudioClip;

        // Prevent object destruction through scenes
        GameObject.DontDestroyOnLoad(gameObject);

        // Check if it is alone, so other sounds should stop
        if (alone)
        {
            StopAllSounds();
        }


        // Store playing sound in a map.
        NonOneShotSounds.Add(sound, gameObject);

        // Configure audioSource
        audioSource.clip = audioClip;
        audioSource.playOnAwake = false;
        audioSource.loop = looping;
        audioSource.volume = options.MusicVolumeClamped;

        // Play sound
        audioSource.Play();
    }

    private void StopAllSounds()
    {
        List<Sound> playingSoundsToRemove = new List<Sound>();
        foreach (var item in NonOneShotSounds.Keys)
        {
            StopSound(item, false);
            playingSoundsToRemove.Add(item);
        }
        foreach (var item in playingSoundsToRemove)
        {
            NonOneShotSounds.Remove(item);
        }
    }

    public void StopSound(Sound sound, bool removeFromDict = true)
    {
        // Find obj if exists
        GameObject gameObject = NonOneShotSounds[sound];
        if (gameObject == null)
        {
            Debug.Log($"Sound {sound} was not playing");
            return;
        }


        // Stop player
        gameObject.GetComponent<AudioSource>().Stop();

        // Remove from map
        if (removeFromDict)
            NonOneShotSounds.Remove(sound);

        // Destroy Gameobject
        GameObject.Destroy(gameObject);

    }

    public void SetVolumeForPlayingSound(Sound sound, float volume)
    {
        GameObject playingSoundObj = NonOneShotSounds[sound];
        if (playingSoundObj != null)
        {
            playingSoundObj.GetComponent<AudioSource>().volume = volume;
        }

    }

    private SoundInfo GetSoundInfo(Sound sound)
    {
        foreach (SoundInfo item in AssetsManager.Instance.Sounds)
        {
            if (item.Sound == sound)
            {
                return item;
            }
        }
        Debug.LogError($"Cannot find audio {sound}");
        return null;
    }
}
