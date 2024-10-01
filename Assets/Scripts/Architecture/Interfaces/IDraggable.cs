using UnityEngine;
using UnityEngine.InputSystem;

public interface IDraggable
{
    void OnDragStart();
    void OnDrag(Vector3 newPosition);
    void OnDragEnd();
    bool IsDragEnabled();
}
