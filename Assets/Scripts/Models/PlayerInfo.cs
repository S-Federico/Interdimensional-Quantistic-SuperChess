using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public List<string> Consumables = new List<string>();
    public List<string> Manuals = new List<string>();
    public List<PieceData> ExtraPieces = new List<PieceData>();
    public List<PieceData> CurrentlyUsedExtraPieces = new List<PieceData>();
    public int Money = 0;
}