using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsManager : MonoBehaviour
{
    public List<SoundInfo> Sounds;
    public static AssetsManager Instance;
    private AssetsManager() {}
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
