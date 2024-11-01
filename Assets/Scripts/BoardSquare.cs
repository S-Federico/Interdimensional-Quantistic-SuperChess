using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour, IClickable
{
    private BoardManager boardManager;
    private Vector2 position;
    public Vector2 Position { get => position; set => position = value; }
    public List<ScriptableStatusModifier> ManualsModifiers = new List<ScriptableStatusModifier>();

    void Awake()
    {
        if (boardManager == null)
        {
            boardManager = FindAnyObjectByType<BoardManager>();
        }
    }

    public void Update()
    {
        if (boardManager == null) return;
        Highlight();

    }

    public void Highlight()
    {
        Renderer renderer = this.gameObject.GetComponent<Renderer>();

        if (boardManager.highlightedSquares.TryGetValue(((int)position.x, (int)position.y), out int result))
        {
            if (renderer != null)
            {
                renderer.enabled = true;
                switch (result)
                {
                    case 1:
                        renderer.material.color = Color.green;
                        break;
                    case 2:
                        renderer.material.color = Color.red;
                        break;
                    case 3:
                        renderer.material.color = Color.black;
                        break;
                    default:
                        renderer.material.color = Color.white;
                        break;
                }
            }
        }
        else
        {
            renderer.enabled = false;
        }
    }
    public void OnClick()
    {
        boardManager.HandleSquareClick(this);
    }

    // Override del metodo Equals per confrontare i BoardSquare in base alla loro Position
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is BoardSquare))
            return false;

        BoardSquare other = obj as BoardSquare;
        return this.Position == other.Position; // Confronto basato sulla posizione
    }

    // Override di GetHashCode (necessario quando si sovrascrive Equals)
    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}
