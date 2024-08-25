using UnityEngine;

[CreateAssetMenu(fileName = "NewBoardState", menuName = "Chess/BoardState")]
public class BoardState : ScriptableObject
{
    public ChessPiece[,] boardState = new ChessPiece[8, 8];
}
