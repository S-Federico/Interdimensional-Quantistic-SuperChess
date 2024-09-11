using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using UnityEngine;

public class PieceStatus : MonoBehaviour, IClickable
{
    public PieceType PieceType;
    public int Hp = 1;
    public int Attack = 1;
    public PieceColor PieceColor;
    public int ID;
    public Vector2 Position;

    //deve essere sempre di dimensioni dispari e con il pezzo al centro
    [SerializeField] public Array2DInt MovementMatrixInfo;

    private int[,] _MovementMatrix;

    private BoardManager boardManager;

    void Start()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        BuildMovementMatrix();
    }

    private void BuildMovementMatrix()
    {
        _MovementMatrix = new int[MovementMatrixInfo.GridSize.x, MovementMatrixInfo.GridSize.y];
        // Compute MovementMatrix from data in editor
        if (MovementMatrixInfo != null)
        {
            for (int i = 0; i < MovementMatrixInfo.GridSize.x; i++)
            {
                for (int j = 0; j < MovementMatrixInfo.GridSize.y; j++)
                {
                    if (this.PieceColor == PieceColor.White)
                    {
                        _MovementMatrix[i, j] = MovementMatrixInfo.GetCell(j, i);

                    }
                    else
                    {
                        _MovementMatrix[i, j] = MovementMatrixInfo.GetCell(MovementMatrixInfo.GridSize.x - j - 1, MovementMatrixInfo.GridSize.y - i - 1);
                    }
                }
            }
        }
    }

    public int[,] MovementMatrix
    {
        get
        {
            if (_MovementMatrix == null)
            {
                BuildMovementMatrix();
            }
            return _MovementMatrix;
        }
    }

    public string Code
    {
        get
        {
            return "" + this.PieceType + this.PieceColor;
        }
    }

    public void TakeDamage(int damage)
    {
        this.Hp -= damage;
    }

    public void OnClick()
    {
        boardManager.SelectPiece(this.gameObject);
    }
}
