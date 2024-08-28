using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum PieceColor{
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

public class Piece{
    
    private PieceType type;
    private int hp;
    private int attack;
    private PieceColor color;
    
    //deve essere sempre di dimensioni dispari e con il pezzo al centro
    private int[,] movementMatrix;

    public Piece(PieceType type,int hp,int attack,PieceColor color,int[,] movementMatrix){
        this.type=type;
        this.hp=hp;
        this.attack=attack;
        this.color=color;
        this.movementMatrix=movementMatrix;
    }

    public int[,] GetMovementMatrix(){
        return this.movementMatrix;
    }
    public new PieceType GetType(){
        return type;
    }

    
    public int GetHP(){
        return hp;
    }
    
    public int GetAttack(){
        return attack;
    }

    public PieceColor GetColor(){
        return color;
    }
    public override string ToString(){
        return "Piece:"+type+","+hp+","+attack+","+color+",";
    }
}

public class ChessBoardModel{

    private Piece[,] board;

    public ChessBoardModel(int righe, int colonne){
        board=new Piece[righe,colonne];
    }
    
    public void PlacePiece(Piece piece, int[] position){
        board[position[0],position[1]]=piece;
    }

    public void PrintBoard(){
        for(int i=0;i<board.GetLength(0);i++){
            Console.Write("Riga "+(i+1)+" :");
            for(int j=0;j<board.GetLength(1);j++){
                if(board[i,j]!=null)
                    Console.Write(board[i,j].GetType()+"\t");
                else
                    Console.Write("N");
            }
            Console.Write("\n");
        }
    }

    public bool IsWhite(int riga,int colonna){
        return board[riga,colonna].GetColor() == PieceColor.White;
    }

    public List<int[]> GetPossibleMovesForPiece(int riga, int colonna)
    {
        List<int[]> moves = new List<int[]>();

        Piece piece = board[riga, colonna];
        if (piece == null)
            return moves;

        int[,] matrix = piece.GetMovementMatrix();
        int offsetRighe = matrix.GetLength(0) / 2;
        int offsetColonne = matrix.GetLength(1) / 2;

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
                    // Se la matrice di movimento segna che ci puoi andare, controlla se è libera
                    if (matrix[i, j] == 1 && board[newRiga, newColonna] == null)
                    {
                        moves.Add(new int[] { newRiga, newColonna });
                    }
                }
            }
        }
        return moves;
    }

}