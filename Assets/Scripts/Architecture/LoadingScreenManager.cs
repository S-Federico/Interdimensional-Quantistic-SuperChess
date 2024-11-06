using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;
    public GameObject m_LoadingScreenObjct;
    public Slider ProgressBar;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {
        m_LoadingScreenObjct.SetActive(false);
    }
}
