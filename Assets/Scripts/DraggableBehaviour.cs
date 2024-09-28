using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableBehaviour : MonoBehaviour, IDraggable
{
    public bool isDragging = false;
    public float elevation=0.5f;
    private Camera mainCamera;

    public Vector3 oldPosition;

    public bool isDraggable;

    void Start(){
        mainCamera = Camera.main;
    }
    public void OnDrag(Vector3 newPosition)
    {

    }

    public void OnDragEnd()
    {
        isDragging = false;
        if (TryGetComponent<ItemData>(out ItemData itemDataComponent))
        {
            //itemDataComponent.OnDragEnd();
        }
        else if (TryGetComponent<PieceStatus>(out PieceStatus pieceStatusComponent))
        {
            pieceStatusComponent.OnDragEnd();
        }
        else
        {
            Debug.LogWarning("Né ItemData né PieceStatus sono stati trovati sul GameObject!");
        }
    }

    public void OnDragStart()
    {
        oldPosition=transform.position;
        transform.position=new Vector3(transform.position.x,transform.position.y+elevation,transform.position.z);
        isDragging = true;
    }

    void Update()
    {
        if (isDragging)
        {
            // Ottieni la posizione del mouse sullo schermo
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // Converti la posizione del mouse sullo schermo in una posizione 3D nel mondo
            // Usa ScreenToWorldPoint per ottenere la posizione nel mondo a una determinata distanza dalla telecamera
            // Assumiamo che draggedObject sia un MonoBehaviour, quindi lo castiamo
            Vector3 screenPosition = new Vector3(mousePosition.x, mousePosition.y, mainCamera.WorldToScreenPoint(transform.position).z);

            // Converti la posizione dello schermo in una posizione nel mondo
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

            // Imposta la nuova posizione dell'oggetto, mantenendo l'asse Y invariato (puoi scegliere l'asse da mantenere fisso)
            Vector3 newPosition = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);

            transform.position = newPosition;
        }
    }

    public bool IsDragEnabled()
    {
        return isDraggable;
    }
}