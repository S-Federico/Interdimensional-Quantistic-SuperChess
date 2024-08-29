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

    private int[] position;
    
    //deve essere sempre di dimensioni dispari e con il pezzo al centro
    private int[,] movementMatrix;

    public Piece(PieceType type,int hp,int attack,PieceColor color,int[,] movementMatrix,int[] position){
        this.type=type;
        this.hp=hp;
        this.attack=attack;
        this.color=color;
        this.movementMatrix=movementMatrix;
        this.position=position;
    }

    public int[,] GetMovementMatrix(){
        return this.movementMatrix;
    }
    public new PieceType GetType(){
        return type;
    }

    public int[] GetPosition(){
        return this.position;
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
        return "Piece: "+type+", "+hp+", "+attack+", "+color+", "+position;
    }
}

public class ChessBoardModel{

    private Piece[,] board;

    public ChessBoardModel(int righe, int colonne){
        board=new Piece[righe,colonne];
    }
    
    public void PlacePiece(Piece piece){
        board[piece.GetPosition()[0],piece.GetPosition()[1]]=piece;
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
        int offsetRighe = (matrix.GetLength(0) / 2)-riga;
        int offsetColonne = (matrix.GetLength(1) / 2)-colonna;
        
        for(int i=0;i<board.GetLength(0);i++){
            for(int j=0;j<board.GetLength(1);j++){
                //Se nessuno dei due indici è negativo o supera la linghezza della matrice di movimento
                if(i + offsetRighe >= 0 && i + offsetRighe < matrix.GetLength(0) &&
                   j + offsetColonne >= 0 && j + offsetColonne < matrix.GetLength(1))
                   {
                    int? move=matrix[i+offsetRighe,j+offsetColonne];
                    switch(move){
                        case null:
                            break;
                        case 1:
                            moves.Add(new int[] { i, j });
                            break;
                    }
                }
            }
        }
        return moves;
    }

    private void Attack(){

    }

}