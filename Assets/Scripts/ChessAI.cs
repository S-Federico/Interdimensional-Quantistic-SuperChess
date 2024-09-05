using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    public PieceStatus[,] board { get; private set; }
    public int[] lastMove { get; private set; } // Salviamo la mossa fatta in questo stato

    public State(PieceStatus[,] board, int[] lastMove)
    {
        this.board = board;
        this.lastMove = lastMove;
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

    public ChessAI(ChessBoardModel cbm)
    {
        Hist = new Stack<State>();
        this.cbm = cbm; // Inizializzazione del modello della scacchiera
    }

    public int[] GetBestMoveFromPosition(PieceStatus[,] board, int depth)
    {
        // Copia la board iniziale
        copiedBoard = CopyBoard(board);

        Think(depth);

        // Ritorna la migliore mossa (pezzo e destinazione)
        return new int[] { (int)bestPiece.Position.x, (int)bestPiece.Position.y, bestMoveX, bestMoveY };
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

                    Move(new int[] { (int)piece.Position.x, (int)piece.Position.y, targetRow, targetCol }, copiedBoard);
                    int thisMoveValue = AlphaBeta(depth - 1, !isMax, alpha, beta);
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

                    Move(new int[] { (int)piece.Position.x, (int)piece.Position.y, targetRow, targetCol }, copiedBoard);
                    int thisMoveValue = AlphaBeta(depth - 1, !isMax, alpha, beta);
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

    private bool isGameOver()
    {
        int currScore = StaticEvaluationFunction();
        return (currScore < -290 || currScore > 290);
    }

    private void Move(int[] move, PieceStatus[,] board)
    {
        Hist.Push(new State(CopyBoard(board), move));

        int startRow = move[0];
        int startCol = move[1];
        int targetRow = move[2];
        int targetCol = move[3];

        PieceStatus movingPiece = board[startRow, startCol];
        if (movingPiece == null) return;

        PieceStatus targetPiece = board[targetRow, targetCol];
        if (targetPiece == null)
        {
            board[targetRow, targetCol] = movingPiece;
            board[startRow, startCol] = null;
            movingPiece.Position = new Vector2(targetRow, targetCol);
        }
        else
        {
            if (movingPiece.PieceColor != targetPiece.PieceColor)
            {
                targetPiece.TakeDamage(movingPiece.Attack);
                if (targetPiece.Hp <= 0)
                {
                    board[targetRow, targetCol] = movingPiece;
                    board[startRow, startCol] = null;
                    movingPiece.Position = new Vector2(targetRow, targetCol);
                }
            }
        }
    }

    public void Undo()
    {
        if (Hist.Count > 0)
        {
            State previousState = Hist.Pop();
            copiedBoard = CopyBoard(previousState.board);
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
                    copiedBoard[i, j] = new PieceStatus
                    {
                        PieceType = board[i, j].PieceType,
                        Hp = board[i, j].Hp,
                        Attack = board[i, j].Attack,
                        PieceColor = board[i, j].PieceColor,
                        ID = board[i, j].ID,
                        Position = board[i, j].Position,
                    };

                    if (board[i, j].MovementMatrix != null)
                    {
                        copiedBoard[i, j].MovementMatrixInfo = board[i, j].MovementMatrixInfo;
                    }
                }
            }
        }
        return copiedBoard;
    }
}
