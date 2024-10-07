using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using UnityEditor.Experimental;
using UnityEngine;
public class BoardData
{
    public Turn currentTurn;
    public PieceData[,] piecesData;

    public BoardData(Turn t, PieceStatus[,] pieces)
    {
        this.currentTurn = t;
        if (pieces != null)
        {
            piecesData = new PieceData[pieces.GetLength(0), pieces.GetLength(1)];
            for (int i = 0; i < pieces.GetLength(0); i++)
            {
                for (int j = 0; j < pieces.GetLength(1); j++)
                {
                    if (pieces[i, j] != null)
                    {
                        this.piecesData[i, j] = pieces[i, j].GetPieceData();
                    }
                }
            }
        }

    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        BoardData other = (BoardData)obj;

        if (currentTurn != other.currentTurn)
            return false;

        if (piecesData.GetLength(0) != other.piecesData.GetLength(0) || piecesData.GetLength(1) != other.piecesData.GetLength(1))
            return false;

        for (int i = 0; i < piecesData.GetLength(0); i++)
        {
            for (int j = 0; j < piecesData.GetLength(1); j++)
            {
                if (piecesData[i, j] != null)
                {
                    if (!piecesData[i, j].Equals(other.piecesData[i, j]))
                        return false;
                }

            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        int hash = currentTurn.GetHashCode();

        foreach (var piece in piecesData)
        {
            hash = HashCode.Combine(hash, piece.GetHashCode());
        }

        return hash;
    }
}

public class PieceData
{
    public PieceType PieceType;
    public int Hp;
    public int Attack;
    public PieceColor PieceColor;
    public int PrefabID;
    public int[] Position = new int[]{};
    public int[,] MovementMatrix = new int[,]{};
    public List<ScriptableStatusModifier> Modifiers = new List<ScriptableStatusModifier>();

    private static Dictionary<PieceType, int> DIFFICULTY_MAP = new Dictionary<PieceType, int>{
        {PieceType.Bishop, 5},
        {PieceType.Knight, 5},
        {PieceType.Pawn, 2},
        {PieceType.Queen, 10},
        {PieceType.Rook, 6},
        {PieceType.King, 10000}
    };

    public double StrenghtValue {
        get {
            return DIFFICULTY_MAP[this.PieceType] * (Modifiers.Count + 1) * 3;
        }
    }

    public PieceData(PieceType pType, int hp, int att, PieceColor pColor, int id, Vector2 posix, int[,] movMatr)
    {
        this.PieceType = pType;
        this.Hp = hp;
        this.Attack = att;
        this.PieceColor = pColor;
        this.PrefabID = id;
        this.MovementMatrix = movMatr;
        int[] pos = new int[2];
        pos[0] = (int)posix.x; 
        pos[1] = (int)posix.y;
        this.Position = pos;
    }

    public override bool Equals(object obj)
    {
        // Verifica se l'oggetto passato è nullo o di un tipo diverso
        if (obj == null || GetType() != obj.GetType())
            return false;

        // Effettua il cast dell'oggetto a PieceData
        PieceData other = (PieceData)obj;

        // Confronta tutti i campi della classe
        return PieceType == other.PieceType &&
               Hp == other.Hp &&
               Attack == other.Attack &&
               PieceColor == other.PieceColor &&
               PrefabID == other.PrefabID &&
               Position[0] == other.Position[0] &&
               Position[1] == other.Position[1] &&
               MovementMatrixEquals(MovementMatrix, other.MovementMatrix);
    }

    private bool MovementMatrixEquals(int[,] matrix1, int[,] matrix2)
    {
        if (matrix1.GetLength(0) != matrix2.GetLength(0) || matrix1.GetLength(1) != matrix2.GetLength(1))
            return false;

        for (int i = 0; i < matrix1.GetLength(0); i++)
        {
            for (int j = 0; j < matrix1.GetLength(1); j++)
            {
                if (matrix1[i, j] != matrix2[i, j])
                    return false;
            }
        }
        return true;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PieceType, Hp, Attack, PieceColor, PrefabID, Position[0], Position[1], MovementMatrix);
    }

    public static PieceData FromPieceStatus(PieceStatus pieceStatus)
    {
        return new PieceData(
            pieceStatus.PieceType,
            pieceStatus.BaseHp,
            pieceStatus.BaseAttack,
            pieceStatus.PieceColor,
            pieceStatus.PrefabID,
            pieceStatus.Position,
            pieceStatus.MovementMatrix
        );
    }
}