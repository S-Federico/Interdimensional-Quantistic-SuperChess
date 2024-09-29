using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OpponentManager : MonoBehaviour
{
    public List<PieceStatus> pieces;
    private ChessAI ai;
    private ChessBoardModel cbm;
    public BoardManager boardManager;

    public void Start()
    {
        cbm = new ChessBoardModel();
        ai = new ChessAI(cbm);
        boardManager = GameObject.FindAnyObjectByType<BoardManager>();

    }

    public void ExecuteAITurn(PieceStatus[,] Pieces)
    {
        // Calcola la migliore mossa con l'IA
        int[] bestMove = ai.GetBestMoveFromPosition(Pieces, 4); // Imposta la profondità desiderata (es. 3)
        if (bestMove == null || bestMove.Length != 4)
        {
            Debug.Log("Nessuna mossa valida trovata dall'IA.");
            // Gestire la fine del gioco o lo stallo
            return;
        }

        if (bestMove.Length == 4)
        {
            int startX = bestMove[0];
            int startY = bestMove[1];
            int endX = bestMove[2];
            int endY = bestMove[3];

            HashSet<int[]> allowedMoves = cbm.GetPossibleMovesForPiece(Pieces[startX,startY], Pieces);

            string moves = "Moves: " + string.Join(",", allowedMoves.Select(move => $"({move[0]},{move[1]})"));

            Debug.Log($"{allowedMoves.Count} Moves found for {Pieces[startX,startY].PieceColor} {Pieces[startX,startY].PieceType}: {moves}");

            Debug.Log($"Start=({startX},{startY})  Finish=({endX},{endY})");

            PieceStatus movingPiece = Pieces[startX, startY];

            boardManager.SelectPiece(movingPiece.gameObject);
            if (Pieces[endX, endY] == null)
            {
                boardManager.MovePiece(boardManager.selectedPiece, new Vector2(endX, endY));
            }
            else
            {
                boardManager.AttackPiece(movingPiece, Pieces[endX, endY]);
            }

            boardManager.selectedPiece = null;

        }

        CleanTempObjects();
    }

    private void CleanTempObjects()
    {
        foreach (GameObject toDestroy in ai.ToDestroy)
        {
            Destroy(toDestroy);
        }
    }

}