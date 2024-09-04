using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour, IClickable
{
    private BoardManager boardManager;
    private Vector2 position;
    public Vector2 Position {get => position; set => position = value;}
    void Awake()
    {
        if (boardManager == null)
        {
            boardManager = FindAnyObjectByType<BoardManager>();
        }
    }

    public void OnClick()
    {
        boardManager.HandleSquareClick(this);
    }
}
