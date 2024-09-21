using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public Camera mainCamera;

    // Input action asset reference
    public InputActionAsset inputActions;
    private InputAction leftClickAction;
    private InputAction rightClickAction;

    void Awake()
    {
        // Assicurati di assegnare la telecamera principale
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Configura le input action per il left click e il right click
        leftClickAction = inputActions.FindActionMap("Gameplay").FindAction("Left click");
        rightClickAction = inputActions.FindActionMap("Gameplay").FindAction("Right click");
    }

    void OnEnable()
    {
        // Abilita le actions quando lo script � attivo
        leftClickAction.Enable();
        leftClickAction.performed += OnLeftClick;

        rightClickAction.Enable();
        rightClickAction.performed += OnRightClick;
    }

    void OnDisable()
    {
        // Disabilita le actions quando lo script � disattivato
        leftClickAction.performed -= OnLeftClick;
        leftClickAction.Disable();

        rightClickAction.performed -= OnRightClick;
        rightClickAction.Disable();
    }

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        HandleClick("left");
    }

    private void OnRightClick(InputAction.CallbackContext context)
    {
        HandleClick("right");
    }

    private void HandleClick(string buttonType)
    {
        //boardManager.SetShowMovesFlag(false);
        // Ottieni la posizione del mouse
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        // Controlla se il raycast colpisce qualcosa
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Hai cliccato con il tasto " + buttonType + " su: " + hit.collider.name + " , " + hit.collider.gameObject.tag);

            // Generization of click
            if (hit.collider.TryGetComponent(out IClickable cliclable))
            {                
                cliclable.OnClick();
            }
        }
    }
}
