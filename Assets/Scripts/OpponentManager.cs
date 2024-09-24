using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OpponentManager : MonoBehaviour
{
    public List<PieceStatus> pieces;
    private ChessAI ai; 
    private ChessBoardModel cbm;

    public void Start()
    {
                cbm = new ChessBoardModel();

                ai = new ChessAI(cbm); 

    }


}