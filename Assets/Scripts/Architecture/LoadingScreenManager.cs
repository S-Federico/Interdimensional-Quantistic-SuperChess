using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Istance;
    public GameObject m_LoadingScreenObjct;
    public Slider ProgressBar;

    void Awake()
    {
        if (Istance != null && Istance != this)
        {
            Destroy(this.gameObject);
            Debug.Log("LoadingScreenManager distrutto");
        }
        else
        {
            Istance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {
        m_LoadingScreenObjct.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
