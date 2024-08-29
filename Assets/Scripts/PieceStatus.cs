using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using UnityEngine;

public class PieceStatus : MonoBehaviour
{
    public char PieceType;
    public int Hp;
    public int Attack;
    public char Color;
    
    //deve essere sempre di dimensioni dispari e con il pezzo al centro
    [SerializeField] private Array2DInt MovementMatrixInfo;

    public int[,] MovementMatrix;

    void Awake() {
        MovementMatrix = new int[MovementMatrixInfo.GridSize.x,MovementMatrixInfo.GridSize.y];
        // Compute MovementMatrix from data in editor
        if (MovementMatrixInfo != null) {
            for (int i = 0; i < MovementMatrixInfo.GridSize.x; i++)
            {
                for (int j = 0; j < MovementMatrixInfo.GridSize.y; j++)
                {
                    MovementMatrix[i,j] = MovementMatrixInfo.GetCell(i,j);
                }
            }
        }


    }

    public string Code {
        get {
            return "" + this.PieceType + this.Color;
        }
    }

    

}
