using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public List<ScriptableConsumable> Consumables = new List<ScriptableConsumable>();
    public List<ScriptableManual> Manuals = new List<ScriptableManual>();
    public List<PieceData> ExtraPieces = new List<PieceData>();
    public List<PieceData> CurrentlyUsedExtraPieces = new List<PieceData>();
    public int Money = 0;
}