using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager
{

    public static void PlaySoundOneShot(Sound sound) {
        GameObject gameObject = new GameObject("SOUND");
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        AudioClip audioClip = GetSoundInfo(sound)?.AudioClip;
        audioSource.PlayOneShot(audioClip);
        
        // Destroy the temporary sound gameObject after it finishes playig
        GameObject.Destroy(gameObject, audioClip.length);
    }

    public static void PlaySoud(Sound sound) {
        //TODO: play music
    }

    private static SoundInfo GetSoundInfo(Sound sound) {
        foreach (SoundInfo item in AssetsManager.Instance.Sounds)
        {
            if (item.Sound == sound) {
                return item;
            }
        }
        Debug.LogError($"Cannot find audio {sound.DisplayName()}");
        return null;
    }
}
