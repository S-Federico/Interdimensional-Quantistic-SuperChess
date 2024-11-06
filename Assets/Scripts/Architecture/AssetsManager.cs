using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsManager : MonoBehaviour
{
    // Assets
    public List<SoundInfo> Sounds;
    public GameObject BooleanOptionPrefab;

    // Singleton Pattern
    public static AssetsManager Instance;
    private GameManager gameManager;
    private AssetsManager() {}
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        // Spawn GameManager
        gameManager = GameManager.Instance;
    }
}
