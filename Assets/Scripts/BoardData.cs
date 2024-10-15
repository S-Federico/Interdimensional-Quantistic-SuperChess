using System;
public class BoardData
{
    public Turn currentTurn;
    public PieceData[,] piecesData;
    public BoardSquareData[,] BoardSquareDatas;

    public BoardData(Turn t, PieceStatus[,] pieces, BoardSquare[,] squares)
    {
        this.currentTurn = t;
        if (pieces != null)
        {
            piecesData = new PieceData[pieces.GetLength(0), pieces.GetLength(1)];
            BoardSquareDatas = new BoardSquareData[pieces.GetLength(0), pieces.GetLength(1)];
            for (int i = 0; i < pieces.GetLength(0); i++)
            {
                for (int j = 0; j < pieces.GetLength(1); j++)
                {
                    if (pieces[i, j] != null)
                    {
                        this.piecesData[i, j] = pieces[i, j].GetPieceData();
                    }
                    BoardSquareDatas[i, j] = BoardSquareData.FromStatus(squares[i,j]);
                }
            }            
            
        }

    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        BoardData other = (BoardData)obj;

        if (currentTurn != other.currentTurn)
            return false;

        if (piecesData.GetLength(0) != other.piecesData.GetLength(0) || piecesData.GetLength(1) != other.piecesData.GetLength(1))
            return false;

        // for (int i = 0; i < piecesData.GetLength(0); i++)
        // {
        //     for (int j = 0; j < piecesData.GetLength(1); j++)
        //     {
        //         if (piecesData[i, j] != null)
        //         {
        //             if (!piecesData[i, j].Equals(other.piecesData[i, j]))
        //                 return false;
        //         }

        //     }
        // }
        if (!Utility.MatrixEquals(this.piecesData, other.piecesData)) return false;

        if (!Utility.MatrixEquals(this.BoardSquareDatas, other.BoardSquareDatas)) return false;


        return true;
    }

    public override int GetHashCode()
    {
        int hash = currentTurn.GetHashCode();

        foreach (var piece in piecesData)
        {
            hash = HashCode.Combine(hash, piece.GetHashCode());
        }

        return hash;
    }
}