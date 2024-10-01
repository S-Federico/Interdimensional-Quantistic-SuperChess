using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Camera mainCamera;
    public InputActionAsset inputActions;

    private InputAction leftClickAction;
    private InputAction rightClickAction;
    private InputAction mousePositionAction;

    private IDraggable draggedObject = null;
    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        leftClickAction = inputActions.FindActionMap("Gameplay").FindAction("Left click");
        rightClickAction = inputActions.FindActionMap("Gameplay").FindAction("Right click");
        mousePositionAction = inputActions.FindActionMap("Gameplay").FindAction("Mouse Position");
    }

    void OnEnable()
    {
        leftClickAction.Enable();
        leftClickAction.performed += OnLeftClick; //Aggiungi questo evento tra i listener di questa azione
        leftClickAction.canceled += OnLeftClickRelease;

        mousePositionAction.Enable();
    }

    void OnDisable()
    {
        leftClickAction.performed -= OnLeftClick;
        leftClickAction.canceled -= OnLeftClickRelease;

        leftClickAction.Disable();
        mousePositionAction.Disable();
    }

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Hai cliccato su: " + hit.collider.name);

            if (hit.collider.TryGetComponent(out IClickable clickable))
            {
                clickable.OnClick();
            }
            if (hit.collider.TryGetComponent(out IDraggable draggable) && draggable.IsDragEnabled())
            {
                draggable.OnDragStart();
                draggedObject = draggable;
            }
        }
    }

    private void OnLeftClickRelease(InputAction.CallbackContext context)
    {
        if (draggedObject != null)
        {
            draggedObject.OnDragEnd();
            draggedObject = null;
        }
    }

}

