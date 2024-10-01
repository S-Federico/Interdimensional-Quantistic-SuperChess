using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LevelGenerator
{

    public static List<PieceStatus> GenerateEnemies(int level, int stage) {
        List<PieceStatus> result = new List<PieceStatus>();
        


        string[] pieces = SaveManager.Instance.FindFilesInPath("Assets/BrokenVector/LowpolyChessPack/Prefabs", "unity");
        List<PieceStatus> possibleBlackPieces = new List<PieceStatus>();
        foreach (var item in pieces)
        {
            GameObject obj = Resources.Load<GameObject>(item);
            possibleBlackPieces.Add(obj.GetComponent<PieceStatus>());
        }
        
        Debug.Log(possibleBlackPieces);

        return result;
    }
}
