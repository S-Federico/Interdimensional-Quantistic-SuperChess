using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;



public class PieceStatus : MonoBehaviour, IClickable
{
    public PieceType PieceType;
/*
    Possibili parametri su cui i modificatori agiscono :

    * Attacco, Hp, numero di mosse sono le cose base

    * cloaked indica se i pezzi avversari possono o meno attaccare questo pezzo

    * ethereal indica se il pezzo blocca o meno i movimenti degli alleati

    * AttackMatrix è una matrice di attacco da applicare nelle caselle attorno al pezzo 
      attaccato, un attacco ad area insomma

    * AoE(Area of effect) indica se fa attacchi ad area o meno, potrebbe essere 
      l'implementazione semplice di quello di prima

    * Cure indica se può curare i pezzi alleati e di quanto

    * Oltre a questo un modificatore potrebbe essere una aggiunta di movimenti alla 
      matrice di movimento, tipo mettere il pezzo a cavallo

    * In realtà si potrebbe fare che un tipo di nemico ha come modificatore i fanti a cavallo, 
      ovvero la cavalleria e quello è un tema

    * Brittle è una proprietà per cui il pezzo diventa di vetro e ha un attacco fortissimo, 
      ma si rompe appena subisce danno

    * Commander significa se aggiunge +1 attacco ai pezzi vicini. 

    * In realtà tutti questi buff possono essere interpretati in questo modo, 
      ovvero che c'è una unità di supporto la cui vicinanza fornisce buff

    * Testuggine significa che vicino a unità dello stesso tipo forma una testuggine e guadagna +1 difesa

    * Potremmo anche pensare di implementare i vari tipi di danni, magici, fuoco, impatto, ecc e 
      in questo modo usare sempre la stessa logica ma calcolare i danni diversamente
      public Enum AttackType { Arcane,Fire,Impact }

    * Potremmo usare una componente fortuna negli attacchi, che possono andare a segno o meno. 
      In questo modo possiamo dare dei buff o debuff a questa probabilità
      
    * Scary riduce la possibilità del nemico di portare a segno l'attacco
*/
    public int Hp = 1;
    public int Attack = 1;
    public int NumberOfMoves = 1;

    public bool cloaked = false;
    public bool ethereal = false;
    public int[,] AttackMatrix;
    public int cure = 0;
    public int[,] AddedMovements;
    public bool brittle = false;
    public bool commander = false;
    public bool testuggine = false;
    public float hitChance=1.0f;
    public bool scary=false;
    public PieceColor PieceColor;
    public int ID;
    public Vector2 Position;

    //deve essere sempre di dimensioni dispari e con il pezzo al centro
    [SerializeField] public Array2DInt MovementMatrixInfo;

    private int[,] _MovementMatrix;

    private BoardManager boardManager;

    void Start()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        BuildMovementMatrix();
    }

    private void BuildMovementMatrix()
    {
        _MovementMatrix = new int[MovementMatrixInfo.GridSize.x, MovementMatrixInfo.GridSize.y];
        // Compute MovementMatrix from data in editor
        if (MovementMatrixInfo != null)
        {
            for (int i = 0; i < MovementMatrixInfo.GridSize.x; i++)
            {
                for (int j = 0; j < MovementMatrixInfo.GridSize.y; j++)
                {
                    if (this.PieceColor == PieceColor.White)
                    {
                        _MovementMatrix[i, j] = MovementMatrixInfo.GetCell(j, i);

                    }
                    else
                    {
                        _MovementMatrix[i, j] = MovementMatrixInfo.GetCell(MovementMatrixInfo.GridSize.x - j - 1, MovementMatrixInfo.GridSize.y - i - 1);
                    }
                }
            }
        }
    }

    public int[,] MovementMatrix
    {
        get
        {
            if (_MovementMatrix == null)
            {
                BuildMovementMatrix();
            }
            return _MovementMatrix;
        }
    }

    public string Code
    {
        get
        {
            return "" + this.PieceType + " " + this.Hp + " " + this.Attack + " " + this.PieceColor + " " + this.ID + " " + this.Position + "\n";
        }
    }

    public PieceData GetPieceData()
    {
        return new PieceData(PieceType, Hp, Attack, PieceColor, ID, Position, _MovementMatrix);
    }

    public void BuildFromData(PieceData pData)
    {
        if (pData != null)
        {
            this.PieceType = pData.PieceType;
            this.Hp = pData.Hp;
            this.Attack = pData.Attack;
            this.PieceColor = pData.PieceColor;
            this.ID = pData.ID;
            this._MovementMatrix = pData.MovementMatrix;
            Vector2 pos = new Vector2(pData.Position[0], pData.Position[1]);
            this.Position = pos;
        }
    }

    public void TakeDamage(int damage)
    {
        this.Hp -= damage;
    }

    public void OnClick()
    {
        boardManager.SelectPiece(this.gameObject);
    }
}
