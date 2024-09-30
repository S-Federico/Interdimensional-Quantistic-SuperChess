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
    public PlayerManager playerManager;

    public void Start()
    {
        cbm = new ChessBoardModel();
        ai = new ChessAI(cbm);
        boardManager = GameObject.FindAnyObjectByType<BoardManager>();
        playerManager = GameObject.FindAnyObjectByType<PlayerManager>();
    }

    public void ExecuteAITurn(PieceStatus[,] Pieces)
    {
        // Calcola la migliore mossa con l'IA
        int[] bestMove = ai.GetBestMoveFromPosition(Pieces, 3,pieces,playerManager.pieces); // Imposta la profondità desiderata (es. 3)
        if (bestMove == null)
        {
            Debug.Log("Nessuna mossa valida trovata dall'IA.");
            // Gestire la fine del gioco o lo stallo
            return;
        }

        if (bestMove[0]!=-1)
        {
            int startX = bestMove[0];
            int startY = bestMove[1];
            int endX = bestMove[2];
            int endY = bestMove[3];

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

        }else{
            int endX = bestMove[2];
            int endY = bestMove[3];
            int pID = bestMove[4];

            boardManager.SelectPiece(FindPiecebyID(pID).gameObject);
            if (Pieces[endX, endY] == null)
            {
                boardManager.MovePiece(boardManager.selectedPiece, new Vector2(endX, endY));
                pieces.Remove(FindPiecebyID(pID));
            }
            else
            {
                Debug.Log("A piece outside the board just attacked one inside! Fix that!");
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

    public PieceStatus FindPiecebyID(int ID){
        PieceStatus result =null;
        foreach(PieceStatus p in pieces){
            if (p.ID == ID)
            result=p;
        }
        return result;
    }


}