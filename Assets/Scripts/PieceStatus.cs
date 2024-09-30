using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using UnityEngine;

public class PieceStatus : MonoBehaviour, IClickable
{
    public PieceType PieceType;

    // Variabili base private
    private int baseHp = 1;
    private int baseAttack = 1;
    private int baseNumberOfMoves = 1;

    // Proprietà calcolate
    public int Hp
    {
        get
        {
            return baseHp + CalculateBuff(appliedModifiers, AttributeType.Hp) + CalculateBuff(CellModifiers, AttributeType.Hp) - DamageTaken;
        }
        set
        {
            baseHp = value;
        }
    }

    public int BaseHp { get => this.baseHp; }

    public int Attack
    {
        get
        {
            return baseAttack + CalculateBuff(appliedModifiers, AttributeType.Attack) + CalculateBuff(CellModifiers, AttributeType.Attack);
        }
        set
        {
            baseAttack = value;
        }
    }

    public int BaseAttack { get => this.baseAttack; }

    public int NumberOfMoves
    {
        get
        {
            return baseNumberOfMoves + CalculateBuff(appliedModifiers, AttributeType.NumberOfMoves) + CalculateBuff(CellModifiers, AttributeType.NumberOfMoves);
        }
        set
        {
            baseNumberOfMoves = value;
        }
    }

    public int BaseNumberOfMoves { get => this.baseNumberOfMoves; }

    public int DamageTaken = 0;
    public bool cloaked = false;
    public bool ethereal = false;
    public int[,] AttackMatrix;
    public int cure = 0;
    public int[,] AddedMovements;
    public bool brittle = false;
    public bool commander = false;
    public bool testuggine = false;
    public double hitChance = 1.0;
    public bool scary = false;

    // Modificatori applicati
    public List<ScriptableStatusModifier> appliedModifiers;

    // Lista di pezzi affected da questo pezzo
    public PieceColor PieceColor;
    public int ID;
    public Vector2 Position;

    // Deve essere sempre di dimensioni dispari e con il pezzo al centro
    [SerializeField] public Array2DInt MovementMatrixInfo;

    private int[,] _MovementMatrix;

    private BoardManager boardManager;
    public List<ScriptableStatusModifier> CellModifiers;

    void Start()
    {
        boardManager = FindAnyObjectByType<BoardManager>();
        BuildMovementMatrix();
        CellModifiers = new List<ScriptableStatusModifier>();
    }

    void Update()
    {
        CellModifiersCheck();
    }

    public int CalculateBuff(List<ScriptableStatusModifier> modifiers, AttributeType type)
    {
        // Facciamo che per ora vanno in ordine di applicazione, poi pensiamo al riordino con priorità
        int result = 0;
        if (modifiers == null)
        {
            return result;
        }
        foreach (ScriptableStatusModifier modifier in modifiers)
        {
            if (modifier.attributeType == type)
            {
                switch (modifier.applicationType)
                {
                    case ModifierApplicationType.Additive:
                        result += (int)modifier.value;
                        break;
                    case ModifierApplicationType.Multiplicative:
                        result *= (int)modifier.value;
                        break;
                    case ModifierApplicationType.Absolute:
                        result = (int)modifier.value;
                        break;

                }
            }
        }
        return result;
    }

    private void CellModifiersCheck()
    {
        GameObject cell = null;
        if (boardManager != null)
        {
            boardManager.GetSquare((int)Position.x, (int)Position.y);
        }
        if (cell != null && cell.GetComponent<BoardSquare>().ManualsModifiers != null)
        {
            CellModifiers = cell.GetComponent<BoardSquare>().ManualsModifiers;
        }
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
        DamageTaken += damage;
    }

    public void CureNearby()
    {
        PieceStatus[,] pieces = boardManager.Pieces;

        Vector2[] directions = new Vector2[]
        {
            new Vector2(0, 1),    // Su
            new Vector2(0, -1),   // Giù
            new Vector2(-1, 0),   // Sinistra
            new Vector2(1, 0),    // Destra
            new Vector2(-1, 1),   // Diagonale Su-Sinistra
            new Vector2(1, 1),    // Diagonale Su-Destra
            new Vector2(-1, -1),  // Diagonale Giù-Sinistra
            new Vector2(1, -1)    // Diagonale Giù-Destra
        };

        foreach (Vector2 direction in directions)
        {
            Vector2 adjacentPosition = Position + direction;
            int x = (int)adjacentPosition.x;
            int y = (int)adjacentPosition.y;

            // Posizione all'interno della matrice
            if (x >= 0 && x < pieces.GetLength(0) && y >= 0 && y < pieces.GetLength(1))
            {
                PieceStatus piece = pieces[x, y];

                if (piece != null)
                {
                    // Applica la cura al pezzo
                    if (piece.DamageTaken >= cure)
                    {
                        piece.DamageTaken -= cure;
                    }
                    else
                    {
                        piece.DamageTaken = 0;
                    }
                }
            }
        }
    }

    public void OnClick()
    {
        boardManager.SelectPiece(this.gameObject);
    }

    public BoardSquare GetSquareBelow()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // Controlla se l'oggetto ha il componente BoardSquare
            BoardSquare boardSquare = hit.collider.gameObject.GetComponent<BoardSquare>();
            if (boardSquare != null)
            {
                return boardSquare;  // Restituisce il componente BoardSquare
            }
        }

        return null;  // Restituisce null se non è stato trovato
    }

    public void OnDragEnd()
    {
        BoardSquare square = GetSquareBelow();
        Debug.Log($"Trying to place piece in ({square.Position.x},{square.Position.y}): {boardManager.CanPlacePiece(this)}");

        if (boardManager.CanPlacePiece(this) && square != null)
        {
            transform.position = square.gameObject.transform.position;
            Position = square.Position;
            if (TryGetComponent<DraggableBehaviour>(out DraggableBehaviour draggableBehaviour))
            {
                draggableBehaviour.isDraggable = false;
                boardManager.PlayerPiecePositioned(this);
            }
        }
        else
        {
            transform.position = gameObject.GetComponent<DraggableBehaviour>().oldPosition;
        }
    }
}
