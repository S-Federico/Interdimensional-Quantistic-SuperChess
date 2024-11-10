using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsManager : MonoBehaviour
{
    // Assets
    [Header("Sounds")]
    public List<SoundInfo> Sounds;
    [Header("Option components")]
    public GameObject BooleanOptionPrefab;
    public GameObject SliderOptionPrefab;

    [Header("UI Components")]
    public GameObject PopupPrefab;
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
