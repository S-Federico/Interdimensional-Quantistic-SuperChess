using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // Private fields
    private Camera Camera;
    private InputManager inputManager;

    // Inspector references
    public Transform CameraFocus;
    public float RotationSpeed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = FindAnyObjectByType<InputManager>();
        Camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPosition(CameraFocus);

        if (inputManager.RightClickPressed)
        {
            {
                Vector2 mouseMovement = inputManager.MouseDelta;
                //Camera.transform.RotateAround(CameraFocus.position, new Vector3(1, 0, 0), mouseMovement.y * Time.deltaTime * RotationSpeed);
                Camera.transform.RotateAround(CameraFocus.position, new Vector3(0, 1, 0), mouseMovement.x * Time.deltaTime * RotationSpeed);
            }
        }

    }

    void LookAtPosition(Transform lookPosition)
    {
        Camera.transform.LookAt(lookPosition);
    }
}
