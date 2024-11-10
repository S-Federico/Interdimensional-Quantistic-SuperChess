using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class PieceData
{
    public PieceType PieceType;
    public int Hp;
    public int Attack;
    public PieceColor PieceColor;
    public int PrefabID;
    public int[] Position = new int[]{};
    public int[,] MovementMatrix = new int[,]{};
    [JsonIgnore] public List<ScriptableStatusModifier> Modifiers = new List<ScriptableStatusModifier>();
    public List<string> AppliedModifierPaths = new List<string>();

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

    public PieceData(PieceType pType, int hp, int att, PieceColor pColor, int id, Vector2 posix, int[,] movMatr, List<ScriptableModifierData> modifiers)
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
        this.AppliedModifierPaths = new List<string>();
        if (modifiers != null) {
            foreach (var item in modifiers)
            {
                this.AppliedModifierPaths.Add(item.name);
            }
        }

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
        if (matrix1 == null && matrix2 == null) return true;
        if (matrix1 == null || matrix2 == null) return false;
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
            pieceStatus.MovementMatrix,
            pieceStatus.appliedModifiers
        );
    }
}