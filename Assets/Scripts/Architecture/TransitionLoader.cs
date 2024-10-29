using UnityEngine;

public class TransitionLoader : MonoBehaviour
{
    public Animator transition;
    private float transitionTime = 2f;

    public float GetTransitionTime(){
        return transitionTime;
    }

}
