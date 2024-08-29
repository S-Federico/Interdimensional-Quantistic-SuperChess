using UnityEngine;

[CreateAssetMenu(fileName = "NewPiece", menuName = "Chess/Piece")]
public class ChessPiece : ScriptableObject
{
    public int Damage;
    public int HP;
    public int NMoves;
    public PieceType Type;
    public PieceColor PieceColor;
}

