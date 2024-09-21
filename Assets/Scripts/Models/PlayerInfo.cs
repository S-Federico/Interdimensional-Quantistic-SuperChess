using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerInfo
{
    List<PieceStatus> Pieces;
    List<ScriptableConsumable> Consumables;
    List<ScriptableManual> Manuals;
    List<ScriptablePiece> ExtraPieces;
    public int Money;
}