using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.UIElements;

public enum PieceColor
{
    Black,
    White
}

public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public class Piece
{

    private PieceType type;
    private int hp;
    private int attack;
    private PieceColor color;

    private int[] position;

    //deve essere sempre di dimensioni dispari e con il pezzo al centro
    private int[,] movementMatrix;

    public Piece(PieceType type, int hp, int attack, PieceColor color, int[,] movementMatrix, int[] position)
    {
        this.type = type;
        this.hp = hp;
        this.attack = attack;
        this.color = color;
        this.movementMatrix = movementMatrix;
        this.position = position;
    }

    public int[,] GetMovementMatrix()
    {
        return this.movementMatrix;
    }
    public new PieceType GetType()
    {
        return type;
    }

    public int[] GetPosition()
    {
        return this.position;
    }

    public int GetHP()
    {
        return hp;
    }

    public int GetAttack()
    {
        return attack;
    }

    public PieceColor GetColor()
    {
        return color;
    }
    public override string ToString()
    {
        return "Piece: " + type + ", " + hp + ", " + attack + ", " + color + ", " + position;
    }
}

public class ChessBoardModel
{

    private Piece[,] board;

    public ChessBoardModel(int righe, int colonne)
    {
        board = new Piece[righe, colonne];
    }

    public void PlacePiece(Piece piece)
    {
        board[piece.GetPosition()[0], piece.GetPosition()[1]] = piece;
    }

    public void PrintBoard()
    {
        for (int i = 0; i < board.GetLength(0); i++)
        {
            Console.Write("Riga " + (i + 1) + " :");
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] != null)
                    Console.Write(board[i, j].GetType() + "\t");
                else
                    Console.Write("N");
            }
            Console.Write("\n");
        }
    }

    public bool IsWhite(int riga, int colonna)
    {
        return board[riga, colonna].GetColor() == PieceColor.White;
    }

    public HashSet<int[]> GetPossibleMovesForPiece(int riga, int colonna)
    {
        HashSet<int[]> moves = new HashSet<int[]>();

        Piece piece = board[riga, colonna];
        if (piece == null)
            return moves;

        int[,] matrix = piece.GetMovementMatrix();
        int offsetRighe = matrix.GetLength(0) / 2;
        int offsetColonne = matrix.GetLength(1) / 2;

        // Creiamo una lista per tracciare le caselle ostruite
        HashSet<(int, int)> caselleOstruite = CalcolaCaselleOstruite(riga, colonna);

        // Scorri sulle righe della matrice di movimento
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            // Scorri sulle colonne della matrice di movimento
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                int newRiga = riga + (i - offsetRighe);
                int newColonna = colonna + (j - offsetColonne);

                // Controlla che la nuova posizione sia all'interno della scacchiera
                if (newRiga >= 0 && newRiga < board.GetLength(0) && newColonna >= 0 && newColonna < board.GetLength(1))
                {
                    // Controlla che la cella non sia ostruita
                    if (caselleOstruite.Contains((newRiga, newColonna)))
                        continue;

                    // Se la matrice di movimento segna che ci puoi andare
                    if (matrix[i, j] == 1)
                    {
                        if (board[newRiga, newColonna] == null)
                        {
                            // Aggiungi come mossa di movimento (tipo = 1)
                            moves.Add(new int[] { newRiga, newColonna, 1 });
                        }
                        else if (board[newRiga, newColonna].GetColor() == PieceColor.Black) // Controlla se è un pezzo avversario
                        {
                            // Aggiungi come mossa di attacco (tipo = 2)
                            moves.Add(new int[] { newRiga, newColonna, 2 });
                        }
                    }
                }
            }
        }

        // Pulizia del set di mosse per assicurarsi che siano connesse al pezzo di partenza con movimenti (tipo = 1)
        moves = CleanDisconnectedMoves(moves, riga, colonna);

        return moves;
    }

    private HashSet<(int, int)> CalcolaCaselleOstruite(int riga, int colonna)
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

                if (board[newRiga, newColonna] != null)
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


    private HashSet<int[]> CleanDisconnectedMoves(HashSet<int[]> moves, int rigaPezzo, int colonnaPezzo)
    {
        HashSet<int[]> connectedMoves = new HashSet<int[]>();
        HashSet<(int, int)> esplorate = new HashSet<(int, int)>();
        Queue<(int, int)> daEsplorare = new Queue<(int, int)>();

        // Iniziamo l'esplorazione dalla posizione del pezzo
        daEsplorare.Enqueue((rigaPezzo, colonnaPezzo));

        while (daEsplorare.Count > 0)
        {
            var mossaCorrente = daEsplorare.Dequeue();
            int rigaCorrente = mossaCorrente.Item1;
            int colonnaCorrente = mossaCorrente.Item2;

            // Aggiungi la mossa corrente al set delle esplorate
            esplorate.Add(mossaCorrente);

            // Esplora le celle adiacenti
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = rigaCorrente + i;
                    int newCol = colonnaCorrente + j;

                    // Cerca una mossa di tipo movimento (tipo = 1) nel set moves
                    if (moves.Any(m => m[0] == newRow && m[1] == newCol && m[2] == 1) && !esplorate.Contains((newRow, newCol)))
                    {
                        daEsplorare.Enqueue((newRow, newCol));
                        connectedMoves.Add(new int[] { newRow, newCol, 1 });
                    }
                }
            }
        }

        // Aggiungi le mosse di attacco che sono connesse tramite movimenti
        foreach (var move in moves)
        {
            if (move[2] == 2) // È una mossa di attacco
            {
                // Verifica se la mossa di attacco è adiacente a una mossa di movimento connessa
                bool isConnected = connectedMoves.Any(m => Math.Abs(m[0] - move[0]) <= 1 && Math.Abs(m[1] - move[1]) <= 1);
                if (isConnected)
                {
                    connectedMoves.Add(move);
                }
            }
        }

        return connectedMoves;
    }

}