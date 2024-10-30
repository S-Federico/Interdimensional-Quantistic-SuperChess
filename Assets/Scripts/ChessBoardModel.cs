using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.UIElements;
using System.Text;

public class ChessBoardModel
{

    private void PrintBoard(PieceStatus[,] board)
    {
        string row = "";

        for (int i = 0 ; i <board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] != null)
                {
                    row += $"{board[i, j].PieceType.ToString()[0]}({board[i, j].PieceColor.ToString()[0]}) ";
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


    public bool IsWhite(int riga, int colonna, PieceStatus[,] board)
    {
        return board[riga, colonna].PieceColor == PieceColor.White;
    }

    public List<BoardSquare> GetAllowedPlacements(PieceStatus piece, PieceStatus[,] board, BoardManager boardManager)
    {
        List<BoardSquare> allowedPlacements = new List<BoardSquare>();

        if (piece.PieceColor == PieceColor.Black)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == null)
                        allowedPlacements.Add(boardManager.GetSquare(i, j).GetComponent<BoardSquare>());
                }
            }
        }
        else
        {
            for (int i = 6; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] == null)
                        allowedPlacements.Add(boardManager.GetSquare(i, j).GetComponent<BoardSquare>());
                }
            }
        }

        return allowedPlacements;
    }

    public HashSet<int[]> GetPossibleMovesForPiece(PieceStatus pieceStatus, PieceStatus[,] board)
    {
        HashSet<int[]> moves = new HashSet<int[]>();
        HashSet<int[]> disconnectedMoves = new HashSet<int[]>();

        if(pieceStatus.Position.x==-1 && pieceStatus.Position.y==-1){
            BoardManager bm= GameObject.Find("BoardManager").GetComponent<BoardManager>();
            List<BoardSquare> allowedPlacements=GetAllowedPlacements(pieceStatus, board,bm);
            foreach(BoardSquare square in allowedPlacements){
                moves.Add(new int[] { (int)square.Position.x, (int)square.Position.y }) ;
            }
            return moves;
        }

        StringBuilder log = new StringBuilder();  // Variabile per accumulare i log

        int riga = (int)pieceStatus.Position.x;
        int colonna = (int)pieceStatus.Position.y;

        PieceStatus piece = board[riga, colonna];
        if (piece == null)
        {
            log.AppendLine($"No piece found at position ({riga},{colonna}). Exiting method.");
            Debug.Log(log.ToString());
            return moves;
        }

        log.AppendLine($"Processing moves for {piece.PieceType} at position ({riga},{colonna})");

        int[,] matrix = piece.MovementMatrix;
        int offsetRighe = matrix.GetLength(0) / 2;
        int offsetColonne = matrix.GetLength(1) / 2;

        HashSet<(int, int)> caselleOstruite = CalcolaCaselleOstruite(riga, colonna, board);

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                int newRiga = riga + (i - offsetRighe);
                int newColonna = colonna + (j - offsetColonne);

                // Verifica se la posizione è dentro la scacchiera
                if (newRiga >= 0 && newRiga < board.GetLength(0) && newColonna >= 0 && newColonna < board.GetLength(1))
                {
                    log.AppendLine($"Evaluating position ({newRiga},{newColonna})");

                    // Verifica se la casella è ostruita
                    if (caselleOstruite.Contains((newRiga, newColonna)))
                    {
                        log.AppendLine($"Position ({newRiga},{newColonna}) is blocked. Skipping.");
                        continue;
                    }

                    // Movimento normale
                    if (matrix[i, j] == 1)
                    {
                        if (board[newRiga, newColonna] == null)
                        {
                            log.AppendLine($"Movement to empty position ({newRiga},{newColonna}) added.");
                            moves.Add(new int[] { newRiga, newColonna, 1 });
                        }
                        else if (board[newRiga, newColonna].PieceColor != pieceStatus.PieceColor)
                        {
                            if (!HasStatusEffect(board[newRiga, newColonna], StatusEffectType.Cloaked))
                            {
                                log.AppendLine($"Attack move to enemy at ({newRiga},{newColonna}) added.");
                                moves.Add(new int[] { newRiga, newColonna, 2 });
                            }
                            else
                            {
                                log.AppendLine($"Enemy at ({newRiga},{newColonna}) is cloaked. Skipping.");
                            }
                        }
                        else
                        {
                            log.AppendLine($"Friendly piece at ({newRiga},{newColonna}). Skipping.");
                        }
                    }
                    // Movimento solo (non attacco)
                    else if (matrix[i, j] == 2)
                    {
                        if (board[newRiga, newColonna] == null)
                        {
                            log.AppendLine($"Movement-only move to empty position ({newRiga},{newColonna}) added.");
                            moves.Add(new int[] { newRiga, newColonna, 1 });
                        }
                        else
                        {
                            log.AppendLine($"Position ({newRiga},{newColonna}) is occupied by {board[newRiga, newColonna].gameObject.name}. Skipping.");
                        }
                    }
                    // Attacco solo
                    else if (matrix[i, j] == 3)
                    {
                        if (board[newRiga, newColonna] != null && board[newRiga, newColonna].PieceColor != pieceStatus.PieceColor)
                        {
                            if (!HasStatusEffect(board[newRiga, newColonna], StatusEffectType.Cloaked))
                            {
                                log.AppendLine($"Attack-only move to enemy at ({newRiga},{newColonna}) added.");
                                moves.Add(new int[] { newRiga, newColonna, 2 });
                            }
                            else
                            {
                                log.AppendLine($"Enemy at ({newRiga},{newColonna}) is cloaked. Skipping.");
                            }
                        }
                        else
                        {
                            log.AppendLine($"No enemy at ({newRiga},{newColonna}). Skipping.");
                        }
                    }
                    // Movimento e attacco sconnesso
                    else if (matrix[i, j] == 4)
                    {
                        if (board[newRiga, newColonna] == null)
                        {
                            log.AppendLine($"Disconnected movement move to empty position ({newRiga},{newColonna}) added.");
                            disconnectedMoves.Add(new int[] { newRiga, newColonna, 1 });
                        }
                        else if (board[newRiga, newColonna].PieceColor != pieceStatus.PieceColor)
                        {
                            if (!HasStatusEffect(board[newRiga, newColonna], StatusEffectType.Cloaked))
                            {
                                log.AppendLine($"Disconnected attack move to enemy at ({newRiga},{newColonna}) added.");
                                disconnectedMoves.Add(new int[] { newRiga, newColonna, 2 });
                            }
                            else
                            {
                                log.AppendLine($"Enemy at ({newRiga},{newColonna}) is cloaked. Skipping.");
                            }
                        }
                        else
                        {
                            log.AppendLine($"Friendly piece at ({newRiga},{newColonna}). Skipping.");
                        }
                    }
                }
                else
                {
                    log.AppendLine($"Position ({newRiga},{newColonna}) is outside the board. Skipping.");
                }
            }
        }

        // Log pre-pulizia
        string preCleanMoves = string.Join(",", moves.Select(move => $"({move[0]},{move[1]},{move[2]})"));
        log.AppendLine($"Moves before cleaning: {preCleanMoves}");

        // Pulizia delle mosse sconnesse
        moves = CleanDisconnectedMoves(moves, riga, colonna, board);
        moves.UnionWith(disconnectedMoves);

        // Log post-pulizia
        string postCleanMoves = string.Join(",", moves.Select(move => $"({move[0]},{move[1]},{move[2]})"));
        log.AppendLine($"Moves after cleaning: {postCleanMoves}");

        // Log finale
        log.AppendLine($"{moves.Count} moves found for {piece.PieceType} at position ({riga},{colonna})");

        // Stampa del log completo in un'unica chiamata
        //Debug.Log(log.ToString());

        return moves;
    }


    private HashSet<(int, int)> CalcolaCaselleOstruite(int riga, int colonna, PieceStatus[,] board)
    {
        HashSet<(int, int)> caselleOstruite = new HashSet<(int, int)>();

        // Direzioni di movimento possibili (su, giù, sinistra, destra, diagonali)
        int[,] direzioni = new int[,] {
        {-1, 0}, {1, 0}, {0, -1}, {0, 1}, // su, giù, sinistra, destra
        {-1, -1}, {-1, 1}, {1, -1}, {1, 1} // diagonali
    };

        for (int d = 0; d < direzioni.GetLength(0); d++)
        {
            int deltaRiga = direzioni[d, 0];
            int deltaColonna = direzioni[d, 1];

            int step = 1;
            while (true)
            {
                int newRiga = riga + step * deltaRiga;
                int newColonna = colonna + step * deltaColonna;

                // Controlla che la nuova posizione sia all'interno della scacchiera
                if (newRiga < 0 || newRiga >= board.GetLength(0) || newColonna < 0 || newColonna >= board.GetLength(1))
                    break;

                if (board[newRiga, newColonna] != null && !HasStatusEffect(board[newRiga, newColonna], StatusEffectType.Ethereal))
                {
                    // Se troviamo un pezzo, blocchiamo tutte le caselle dietro di esso in questa direzione
                    step++;
                    while (true)
                    {
                        int blockRiga = riga + step * deltaRiga;
                        int blockColonna = colonna + step * deltaColonna;

                        if (blockRiga < 0 || blockRiga >= board.GetLength(0) || blockColonna < 0 || blockColonna >= board.GetLength(1))
                            break;

                        caselleOstruite.Add((blockRiga, blockColonna));
                        step++;
                    }
                    break; // Interrompiamo la ricerca in questa direzione
                }

                step++;
            }
        }

        return caselleOstruite;
    }


    private HashSet<int[]> CleanDisconnectedMoves(HashSet<int[]> moves, int rigaPezzo, int colonnaPezzo, PieceStatus[,] board)
    {
        // Converti il set di mosse da int[] a tuple
        HashSet<(int, int, int)> movesTuple = new HashSet<(int, int, int)>(
            moves.Select(m => (m[0], m[1], m[2]))
        );

        HashSet<(int, int, int)> connectedMoves = new HashSet<(int, int, int)>();
        HashSet<(int, int)> esplorate = new HashSet<(int, int)>();
        Queue<(int, int)> daEsplorare = new Queue<(int, int)>();

        // Iniziamo l'esplorazione dalla posizione del pezzo
        daEsplorare.Enqueue((rigaPezzo, colonnaPezzo));
        esplorate.Add((rigaPezzo, colonnaPezzo)); // Aggiorna esplorate qui

        while (daEsplorare.Count > 0)
        {
            var mossaCorrente = daEsplorare.Dequeue();
            int rigaCorrente = mossaCorrente.Item1;
            int colonnaCorrente = mossaCorrente.Item2;

            // Esplora le celle adiacenti
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = rigaCorrente + i;
                    int newCol = colonnaCorrente + j;

                    // Cerca una mossa di tipo movimento (tipo = 1) nel set moves
                    if (movesTuple.Contains((newRow, newCol, 1)) && !esplorate.Contains((newRow, newCol)))
                    {
                        esplorate.Add((newRow, newCol)); // Aggiorna esplorate
                        daEsplorare.Enqueue((newRow, newCol));
                        connectedMoves.Add((newRow, newCol, 1));
                    }
                }
            }
        }

        // Aggiungi le mosse di attacco che sono connesse tramite movimenti
        foreach (var move in movesTuple)
        {
            if (move.Item3 == 2) // È una mossa di attacco
            {
                // Verifica se la mossa di attacco è adiacente a una mossa di movimento connessa 
                bool isConnected = connectedMoves.Any(m => Math.Abs(m.Item1 - move.Item1) <= 1 && Math.Abs(m.Item2 - move.Item2) <= 1)
                                || (Math.Abs(rigaPezzo - move.Item1) <= 1 && Math.Abs(colonnaPezzo - move.Item2) <= 1);

                if (isConnected)
                {
                    connectedMoves.Add(move);
                }
            }
        }

        // Converti il set di mosse connesse da tuple a int[]
        HashSet<int[]> result = new HashSet<int[]>(
            connectedMoves.Select(m => new int[] { m.Item1, m.Item2, m.Item3 })
        );

        return result;
    }



    public bool HasStatusEffect(PieceStatus piece, StatusEffectType statusEffect)
    {
        bool hasEffect = false;
        if (piece.appliedModifiers != null)
        {
            foreach (ScriptableModifierData status in piece.appliedModifiers)
            {
                if (status.statusEffectType == statusEffect)
                    hasEffect = true;
            }
        }
        if (piece.CellModifiers != null)
        {
            foreach (ScriptableStatusModifier status in piece.CellModifiers)
            {
                if (status.statusEffectType == statusEffect)
                    hasEffect = true;
            }
        }
        return hasEffect;
    }


}