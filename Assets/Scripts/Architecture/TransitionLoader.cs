using UnityEngine;

public class TransitionLoader : MonoBehaviour
{
    public static TransitionLoader Instance;
    public Animator transition;
    private float transitionTime = 0.2f;
    public float AnimationSpeed = 0.2f;

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

    void Update()
    {
        transition.SetFloat("Speed", AnimationSpeed);
    }
}
