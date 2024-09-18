using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public int[] lastMove { get; private set; } // Salviamo la mossa fatta in questo stato

    public PieceStatus movedPiece { get; private set; }
    public PieceStatus capturedPiece { get; set; }
    public bool hasMoved = false;
    public State() { }
    public State(int[] lastMove, PieceStatus movedPiece)
    {
        this.capturedPiece = null;
        this.lastMove = lastMove;
        this.movedPiece = movedPiece;
    }
}

public class ChessAI
{
    PieceStatus[,] copiedBoard;
    private Stack<State> Hist;
    private int maxDepth;
    private int winningValue = 0;
    private ChessBoardModel cbm; // Variabile di istanza per ChessBoardModel
    private PieceStatus bestPiece; // Variabile per salvare il miglior pezzo
    private int bestMoveX, bestMoveY; // Variabili per salvare la miglior mossa
    private List<GameObject> toDestroy;
    public List<GameObject> ToDestroy {get => toDestroy;}
    public ChessAI(ChessBoardModel cbm)
    {
        Hist = new Stack<State>();
        toDestroy = new List<GameObject>();
        this.cbm = cbm; // Inizializzazione del modello della scacchiera
    }

    public int[] GetBestMoveFromPosition(PieceStatus[,] board, int depth)
    {
        // Copia la board iniziale
        copiedBoard = CopyBoard(board);

        Think(depth);

        int[] move = new int[] { (int)bestPiece.Position.x, (int)bestPiece.Position.y, bestMoveX, bestMoveY };

        Debug.Log($"History stack count after AlphaBeta: {Hist.Count}");

        /*
        foreach(GameObject obj in toDestroy){
            if(obj != null){
                Destroy(obj);
            }
        }*/

        // Ritorna la migliore mossa (pezzo e destinazione)
        PrintBoard(copiedBoard);
        Debug.Log($"RigaStart:{(int)bestPiece.Position.x} ColonnaStart{(int)bestPiece.Position.y}");

        return move;
    }

    private void Think(int d)
    {
        maxDepth = d;
        int depth = maxDepth - 1;
        winningValue = AlphaBeta(depth, true, System.Int32.MinValue, System.Int32.MaxValue);
    }

    private int AlphaBeta(int depth, bool isMax, int alpha, int beta)
    {
        // Se la profondità è 0 o il gioco è finito
        if (depth == 0 || isGameOver())
        {
            return StaticEvaluationFunction();
        }

        if (isMax) // Turno AI (pezzi neri)
        {
            int hValue = System.Int32.MinValue;

            foreach (var piece in copiedBoard)
            {
                if (piece == null || piece.PieceColor != PieceColor.Black) continue; // Ignora celle vuote e pezzi bianchi

                HashSet<int[]> allowedMoves = cbm.GetPossibleMovesForPiece(piece, copiedBoard);

                foreach (var move in allowedMoves)
                {
                    int targetRow = move[0];
                    int targetCol = move[1];
                    Debug.Log($"Black testing move: ({piece.Position.x}, {piece.Position.y}) -> ({targetRow}, {targetCol})");

                    Move(new int[] { (int)piece.Position.x, (int)piece.Position.y, targetRow, targetCol }, copiedBoard);
                    int thisMoveValue = AlphaBeta(depth - 1, !isMax, alpha, beta);
                    Debug.Log($"Value:{thisMoveValue}");

                    Undo();

                    if (hValue < thisMoveValue)
                    {
                        hValue = thisMoveValue;
                        if (depth == maxDepth - 1)
                        {
                            bestPiece = piece;
                            bestMoveX = targetRow;
                            bestMoveY = targetCol;
                        }
                    }

                    alpha = Mathf.Max(alpha, hValue);
                    if (beta <= alpha) break;
                }
            }
            return hValue;
        }
        else // Turno del giocatore (pezzi bianchi)
        {
            int hValue = System.Int32.MaxValue;

            foreach (var piece in copiedBoard)
            {
                if (piece == null || piece.PieceColor != PieceColor.White) continue; // Ignora celle vuote e pezzi neri

                HashSet<int[]> allowedMoves = cbm.GetPossibleMovesForPiece(piece, copiedBoard);

                foreach (var move in allowedMoves)
                {
                    int targetRow = move[0];
                    int targetCol = move[1];
                    Debug.Log($"White testing move: ({piece.Position.x}, {piece.Position.y}) -> ({targetRow}, {targetCol})");

                    Move(new int[] { (int)piece.Position.x, (int)piece.Position.y, targetRow, targetCol }, copiedBoard);
                    int thisMoveValue = AlphaBeta(depth - 1, !isMax, alpha, beta);
                    Debug.Log($"Value:{thisMoveValue}");

                    Undo();

                    if (hValue > thisMoveValue)
                    {
                        hValue = thisMoveValue;
                    }

                    beta = Mathf.Min(beta, hValue);
                    if (beta <= alpha) break;
                }
            }
            return hValue;
        }
    }

    private int StaticEvaluationFunction()
    {
        int TotalScore = 0;
        int curr = 0;
        foreach (PieceStatus chessman in copiedBoard)
        {
            if (chessman == null) continue;

            if (chessman.PieceType == PieceType.King)
                curr = 900;
            else if (chessman.PieceType == PieceType.Queen)
                curr = 90;
            else if (chessman.PieceType == PieceType.Rook)
                curr = 50;
            else if (chessman.PieceType == PieceType.Bishop || chessman.PieceType == PieceType.Knight)
                curr = 30;
            else if (chessman.PieceType == PieceType.Pawn)
                curr = 10;

            if (chessman.PieceColor == PieceColor.White)
                TotalScore -= curr;
            else
                TotalScore += curr;
        }
        return TotalScore;
    }

    public bool isGameOver()
    {
        int kingCount = 0;

        foreach (PieceStatus piece in copiedBoard)
        {
            if (piece != null && piece.PieceType == PieceType.King)
            {
                kingCount++;
                // Se ci sono più di un re, non è game over (serve per testing)
                if (kingCount > 1)
                    return false;
            }
        }

        // Se c'è esattamente un re, il gioco è finito
        return kingCount == 1;
    }

    private void Move(int[] move, PieceStatus[,] board)
    {

        int startRow = move[0];
        int startCol = move[1];
        int targetRow = move[2];
        int targetCol = move[3];
        PieceStatus movingPiece = board[startRow, startCol];
        State state = new State(move, movingPiece);

        if (movingPiece == null) return;

        PieceStatus targetPiece = board[targetRow, targetCol];
        if (targetPiece == null)
        {
            board[targetRow, targetCol] = movingPiece;
            board[startRow, startCol] = null;
            movingPiece.Position = new Vector2(targetRow, targetCol);
            state.hasMoved = true;
        }
        else
        {
            if (movingPiece.PieceColor != targetPiece.PieceColor)
            {
                targetPiece.TakeDamage(movingPiece.Attack);

                if (targetPiece.Hp <= 0)
                {
                    state.capturedPiece = targetPiece;
                    board[targetRow, targetCol] = movingPiece;
                    board[startRow, startCol] = null;
                    movingPiece.Position = new Vector2(targetRow, targetCol);
                    state.hasMoved = true;
                }
            }
        }

        Hist.Push(state);

    }

    private void PrintBoard(PieceStatus[,] board)
    {
        string row = "";

        // Invertiamo l'ordine delle righe, partendo dall'ultima
        for (int i = board.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[j, i] != null)
                {
                    row += $"{board[j, i].PieceType.ToString()[0]}({board[j, i].PieceColor.ToString()[0]}) ";
                }
                else
                {
                    row += "[       ] "; // Celle vuote
                }
            }
            row += "\n";
        }
        Debug.Log(row);
    }


    public void Undo()
    {
        if (Hist.Count > 0)
        {
            State previousState = Hist.Pop();

            int[] move = previousState.lastMove;
            PieceStatus movedPiece = previousState.movedPiece;
            PieceStatus capturedPiece = previousState.capturedPiece;
            bool hasMoved=previousState.hasMoved;

            int startRow = move[0];
            int startCol = move[1];
            int targetRow = move[2];
            int targetCol = move[3];

            if(hasMoved){
                copiedBoard[startRow,startCol]=movedPiece;
                movedPiece.Position=new Vector2(startRow,startCol);
                copiedBoard[targetRow,targetCol]=null;
                if(capturedPiece != null){
                    copiedBoard[targetRow,targetCol]=capturedPiece;
                    capturedPiece.Hp+=movedPiece.Attack;
                }
            }else{
                copiedBoard[targetRow,targetCol].Hp+=movedPiece.Attack;
            }

        }
    }

    private PieceStatus[,] CopyBoard(PieceStatus[,] board)
    {
        int rows = board.GetLength(0);
        int cols = board.GetLength(1);
        PieceStatus[,] copiedBoard = new PieceStatus[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (board[i, j] != null)
                {
                    GameObject temp = new GameObject("temp");
                    toDestroy.Add(temp);

                    PieceStatus copy = temp.AddComponent<PieceStatus>();

                    copy.PieceType = board[i, j].PieceType;
                    copy.Hp = board[i, j].Hp;
                    copy.Attack = board[i, j].Attack;
                    copy.PieceColor = board[i, j].PieceColor;
                    copy.ID = board[i, j].ID;

                    // Copia profonda della posizione
                    copy.Position = new Vector2(board[i, j].Position.x, board[i, j].Position.y);

                    copy.MovementMatrixInfo = board[i, j].MovementMatrixInfo;

                    copiedBoard[i, j] = copy;
                }
            }
        }
        return copiedBoard;
    }

}
