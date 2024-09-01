using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerInfo", menuName = "Chess/PlayerInfo")]
public class GameInfo : ScriptableObject
{
    public List<ChessPiece> ChessPieces; 
}