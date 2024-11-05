using UnityEngine;

public class TransitionLoader : MonoBehaviour
{
    public Animator transition;
    private float transitionTime = 1f;

    public float GetTransitionTime(){
        return transitionTime;
    }

}
