using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using TMPro;
using UnityEngine;

public class PieceStatus : MonoBehaviour, IClickable
{
    public PieceType PieceType;

    // Variabili base private
    [SerializeField] private int baseHp = 1;
    [SerializeField] private int baseAttack = 1;
    private int baseNumberOfMoves = 1;
    private ParticleSystem auraParticle;

    // private Renderer modelRenderer;
    // private Color originalColor;
    // public Color highlightColor = Color.yellow; // Change this to your desired highlight color


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
    public List<ScriptableModifierData> appliedModifiers;

    // Lista di pezzi affected da questo pezzo
    public PieceColor PieceColor;
    public int PrefabID;
    public Vector2 Position;

    // Deve essere sempre di dimensioni dispari e con il pezzo al centro
    [SerializeField] public Array2DInt MovementMatrixInfo;

    private int[,] _MovementMatrix;

    private BoardManager boardManager;
    public List<ScriptableStatusModifier> CellModifiers;
    private static int nextID = 0; // Contatore statico condiviso da tutte le istanze

    public int ID { get; set; } // ID pubblico per identificare univocamente il pezzo

    //Ancore per i bottoni
    Vector3 topCenter;
    public GameObject stats;

    void Awake()
    {
        AssignUniqueID();
    }

    private void AssignUniqueID()
    {
        ID = nextID;
        nextID++;
    }

    void Start()
    {
        // modelRenderer = GetComponent<Renderer>();
        // originalColor = modelRenderer.material.color;
        auraParticle = GetComponentInChildren<ParticleSystem>();
        auraParticle?.Stop();
        boardManager = FindAnyObjectByType<BoardManager>();
        BuildMovementMatrix();
        CellModifiers = new List<ScriptableStatusModifier>();
        SetStatsTag();
    }

    void Update()
    {
        CellModifiersCheck();
        UpdateTagText();
    }

    public void SetStatsTag()
    {
        // Calcola il bounding box del modello del GameObject padre (assumendo che il modello abbia un MeshRenderer)
        Renderer modelRenderer = this.gameObject.GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
        {
            float objHeight = modelRenderer.bounds.size.y;
            float objWidth = modelRenderer.bounds.size.x;
            float objLenght = modelRenderer.bounds.size.z;

            topCenter = new Vector3();
            topCenter.z = this.gameObject.transform.position.z;
            topCenter.y = this.gameObject.transform.position.y + objHeight;
            topCenter.x = this.gameObject.transform.position.x;
        }
        else
        {
            Debug.Log("Non è stato trovato un Renderer nel GameObject padre o nei suoi figli.");
        }
        stats = Instantiate(Resources.Load<GameObject>("StatsPrefab"));
        stats.transform.position = this.gameObject.transform.position;
        stats.transform.position = topCenter;

        // Imposta il padre del bottone
        stats.transform.SetParent(transform);
        stats.SetActive(false);

        UpdateTagText();
    }

    public void UpdateTagText()
    {
        //Imposta il testo
        stats.GetComponentInChildren<TextMeshProUGUI>().text = $"A:{this.Attack} / H:{this.Hp}";
    }

    public int CalculateBuff(List<ScriptableModifierData> modifiers, AttributeType type)
    {
        // Facciamo che per ora vanno in ordine di applicazione, poi pensiamo al riordino con priorità
        int result = 0;
        if (modifiers == null)
        {
            return result;
        }
        foreach (ScriptableModifierData modifier in modifiers)
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
        if (Position.x < 0 || Position.y < 0)
            return;
        GameObject cell = null;
        if (boardManager != null)
        {
            cell = boardManager.GetSquare((int)Position.x, (int)Position.y);
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
        return new PieceData(PieceType, BaseHp, BaseAttack, PieceColor, PrefabID, Position, _MovementMatrix, appliedModifiers);
    }

    public void BuildFromData(PieceData pData)
    {
        if (pData != null)
        {
            this.PieceType = pData.PieceType;
            this.Hp = pData.Hp;
            this.Attack = pData.Attack;
            this.PieceColor = pData.PieceColor;
            this.PrefabID = pData.PrefabID;
            this._MovementMatrix = pData.MovementMatrix;
            Vector2 pos = new Vector2(pData.Position[0], pData.Position[1]);
            this.Position = pos;

            this.appliedModifiers = new List<ScriptableModifierData>();
            if (pData.AppliedModifierPaths != null)
            {
                foreach (var item in pData.AppliedModifierPaths)
                {
                    this.appliedModifiers.Add(ScriptableModifierData.FromScriptableObject(Resources.Load<ScriptableStatusModifier>($"{Constants.MODIFIERS_BASE_PATH}/{item}")));
                }
            }

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
        if (square != null)
        {
            Debug.Log($"Trying to place piece in ({square.Position.x},{square.Position.y}): {boardManager.CanPlacePiece(this)}");
        }
        else
        {
            Debug.Log("Square null!");
        }
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

    void OnMouseEnter()
    {
        // Do nothing if game is paused
        if (GameManager.Instance.IsPaused) return;

        // Change the color when the mouse enters the model
        //modelRenderer.material.color = highlightColor;
        if (auraParticle != null)
        {
            //auraParticle.Play();
            stats.SetActive(true);
        }
        GameUI.SetCursor(CursorType.Hand);
    }

    void OnMouseExit()
    {
        // Do nothing if game is paused
        if (GameManager.Instance.IsPaused) return;

        // Revert back to the original color when the mouse exits
        //modelRenderer.material.color = originalColor;
        if (auraParticle != null)
        {
            //auraParticle.Stop();
            stats.SetActive(false);
        }
        GameUI.SetCursor(CursorType.Default);
    }
}
