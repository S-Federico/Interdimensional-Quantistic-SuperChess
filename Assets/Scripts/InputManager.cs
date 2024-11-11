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

    // Right click
    private bool rightClickPressed = false;
    public bool RightClickPressed { get => rightClickPressed; }

    // Mouse movement
    private Vector2 mouseDelta = Vector2.zero;
    private Vector2? currMousePosition = null;
    public Vector2 MouseDelta { get => mouseDelta; }

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

        // Read right click pression value
        rightClickAction.performed += ctx => rightClickPressed = ctx.ReadValue<float>() > 0.0f;
        rightClickAction.canceled += ctx => rightClickPressed = ctx.ReadValue<float>() > 0.0f;

        OnMouseMove();
        // mousePositionAction.performed += OnMouseMove();
        // mousePositionAction.canceled += ctx => mouseDelta = Vector2.zero;
    }

    void OnEnable()
    {
        leftClickAction.Enable();
        rightClickAction.Enable();
        leftClickAction.performed += OnLeftClick; //Aggiungi questo evento tra i listener di questa azione
        leftClickAction.canceled += OnLeftClickRelease;

        mousePositionAction.Enable();
    }

    void OnDisable()
    {
        leftClickAction.performed -= OnLeftClick;
        leftClickAction.canceled -= OnLeftClickRelease;

        leftClickAction.Disable();
        rightClickAction.Disable();
        mousePositionAction.Disable();
    }

    private void OnCheat()
    {
        PieceStatus[] pieces = FindObjectsByType<PieceStatus>(FindObjectsSortMode.None);
        foreach (var piece in pieces)
        {
            if (PieceColor.Black == piece.PieceColor && PieceType.King == piece.PieceType) {
                Destroy(piece.gameObject);
                break;
            }
        }

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

    void Update()
    {
        OnMouseMove();
    }

    private void OnMouseMove()
    {
        if (!mousePositionAction.enabled) return;
        Vector2 newPos = mousePositionAction.ReadValue<Vector2>();
        if (currMousePosition != null)
        {
            mouseDelta = (Vector2)(newPos - currMousePosition);
        }
        currMousePosition = newPos;
    }
}

