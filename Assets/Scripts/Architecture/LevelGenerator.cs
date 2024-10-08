using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : Singleton<LevelGenerator>
{
    private const int BASE_DIFFICULTY = 10;
    private const int VARIABLE_BASE_DIFFICULTY = 2;

    /// <summary>
    /// This function creates a list of PieceStatus, taken from prefabs, to load on a given level at a given stage
    /// </summary>
    /// <param name="level">The level to generate enemies for</param>
    /// <param name="stage">The stage of the level to generat enemies for</param>
    /// <returns></returns>
    public List<PieceData> GeneratePieces(string piecespath, string modpath, PieceColor color, int level = 1, int stage = 1, int maxPieces = 8)
    {
        List<PieceData> result = new List<PieceData>();

        GameObject[] prefabs = Resources.LoadAll<GameObject>(piecespath);
        ScriptableStatusModifier[] ModifierPrefabs = Resources.LoadAll<ScriptableStatusModifier>(modpath);

        List<PieceData> pieces = new List<PieceData>();

        foreach (var piece in prefabs)
        {
            if (piece.TryGetComponent<PieceStatus>(out PieceStatus pieceStatus) && pieceStatus.PieceColor == color)
            {
                pieces.Add(pieceStatus.GetPieceData());
            }
        }

        // Always add King
        PieceData king = pieces.First(p => p.PieceType == PieceType.King);


        // Remove King from possible pieces
        pieces.RemoveAll(p => p.PieceType == PieceType.King);

        double difficultyToReach = BASE_DIFFICULTY + Math.Pow(VARIABLE_BASE_DIFFICULTY, level * stage);
        double currentDifficulty = 0.0f;

        while (currentDifficulty < difficultyToReach && result.Count < maxPieces && pieces.Count > 0)
        {
            int pieceIndex = (int)UnityEngine.Random.Range(0, pieces.Count);
            PieceData randomPiece = pieces[pieceIndex];
            result.Add(randomPiece);
            currentDifficulty += randomPiece.StrenghtValue;
            pieces.RemoveAt(pieceIndex);
        }

        if (currentDifficulty < difficultyToReach)
        {
            while (currentDifficulty < difficultyToReach)
            {
                PieceData randomPiece = result[(int)UnityEngine.Random.Range(0, result.Count)];
                currentDifficulty -= randomPiece.StrenghtValue;

                ScriptableStatusModifier randomScriptableStatusModifier = ModifierPrefabs[(int)UnityEngine.Random.Range(0, ModifierPrefabs.Length)];
                if (randomScriptableStatusModifier.modifierType == ModifierType.SpecialEffect && randomPiece.Modifiers.Find(m => m.name == randomScriptableStatusModifier.name) != null)
                {
                    currentDifficulty += randomPiece.StrenghtValue;
                    continue;
                }
                //NB: Qui sto passando il prefab!!
                randomPiece.Modifiers.Add(Instantiate(randomScriptableStatusModifier));
                currentDifficulty += randomPiece.StrenghtValue;
            }
        }

        return result;
    }

    public BoardData GenerateDefaultBoardData()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Pieces");
        PieceStatus whiteKing = null;
        PieceStatus blackKing = null;
        foreach (var piece in prefabs)
        {
            if (whiteKing != null && blackKing != null) break;
            if (piece.TryGetComponent<PieceStatus>(out PieceStatus pieceStatus) && pieceStatus.PieceColor == PieceColor.Black && pieceStatus.PieceType == PieceType.King)
            {
                blackKing = pieceStatus;
            }
            else if (piece.TryGetComponent<PieceStatus>(out PieceStatus pieceStatus2) && pieceStatus.PieceColor == PieceColor.White && pieceStatus.PieceType == PieceType.King)
            {
                whiteKing = pieceStatus2;
            }
        }
        PieceStatus[,] pieces = new PieceStatus[8, 8];
        pieces[7, 4] = whiteKing;
        pieces[0, 4] = blackKing;
        BoardData boardData = new BoardData(Turn.Player, pieces);
        return boardData;
    }
}
