using UnityEngine;

public class TransitionLoader : MonoBehaviour
{
    public static TransitionLoader Instance;
    public Animator transition;
    private float transitionTime = 1f;

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

    public float GetTransitionTime()
    {
        return transitionTime;
    }

}
