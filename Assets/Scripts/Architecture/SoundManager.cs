using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager
{

    private static Dictionary<Sound, GameObject> NonOneShotSounds = new Dictionary<Sound, GameObject>();

    public static void PlaySoundOneShot(Sound sound)
    {
        GameObject gameObject = new GameObject("SOUND");
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        AudioClip audioClip = GetSoundInfo(sound)?.AudioClip;
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
    public static void PlaySoud(Sound sound, bool looping = true, bool alone = false)
    {
        GameObject gameObject = new GameObject($"Sound_{sound}");
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        AudioClip audioClip = GetSoundInfo(sound)?.AudioClip;

        // Prevent object destruction through scenes
        GameObject.DontDestroyOnLoad(gameObject);

        // Check if it is alone, so other sounds should stop
        if (alone) {
            foreach (var item in NonOneShotSounds.Keys)
            {
                StopSound(item);
            }
        }

        // Store playing sound in a map.
        NonOneShotSounds.Add(sound, gameObject);

        // Configure audioSource
        audioSource.clip = audioClip;
        audioSource.playOnAwake = false;
        audioSource.loop = looping;

        // Play sound
        audioSource.Play();



    }

    public static void StopSound(Sound sound)
    {
        // Find obj if exists
        GameObject gameObject = NonOneShotSounds[sound];
        if (gameObject == null) {
            Debug.Log($"Sound {sound} was not playing");
            return;
        }


        // Stop player
        gameObject.GetComponent<AudioSource>().Stop();
        
        // Remove from map
        NonOneShotSounds.Remove(sound);
        
        // Destroy Gameobject
        GameObject.Destroy(gameObject);

    }

    private static SoundInfo GetSoundInfo(Sound sound)
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
