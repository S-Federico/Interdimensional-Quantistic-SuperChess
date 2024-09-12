using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using System.Linq;


public class BoardManager : MonoBehaviour
{
    private Turn currentTurn = Turn.Player; // Iniziamo col turno del giocatore
    private ChessAI ai; // Aggiungi questa dichiarazione in cima


    [SerializeField] private GameObject board;         // Riferimento al GameObject "Board" che contiene il piano
    public Array2DInt BoardData;
    public List<GameObject> PiecePrefabs;
    private PieceStatus[,] Pieces;
    private int Riga;
    private int Colonna;

    private Transform planeTransform;                 // Riferimento al Transform del Piano
    private float squareSize;                         // Dimensione di una singola casella
    private int boardSize = 8;                        // Dimensione della scacchiera (8x8)
    private GameObject[,] squares;                    // Array bidimensionale per memorizzare le caselle

    private ChessBoardModel cbm;

    private bool showMovesFlag;
    private bool highlightedFlag;
    private bool alreadyExcecuting;

    public GameObject selectedPiece;

    void Start()
    {
        cbm = new ChessBoardModel();
        ai = new ChessAI(cbm); // Inizializza l'IA con il modello della scacchiera
        InitializeBoard();

        showMovesFlag = false;
        alreadyExcecuting = false;

        // Questa riga di codice carica i pezzi da inspector
        Pieces = LoadBoardFromBoardData();
    }


    void Update()
    {
        if (currentTurn == Turn.Player)
        {
            alreadyExcecuting = false;

            // Gestione input del giocatore
            if (showMovesFlag)
            {
                if (selectedPiece != null)
                {
                    HighlightMoves();
                }
                else
                {
                    HideMoves();
                }
            }
        }
        else if (currentTurn == Turn.AI)
        {
            if (!alreadyExcecuting)
            {
                alreadyExcecuting = true;
                ExecuteAITurn();
            }
        }

        showMovesFlag = false;
    }
    private void ExecuteAITurn()
    {
        // Calcola la migliore mossa con l'IA
        int[] bestMove = ai.GetBestMoveFromPosition(Pieces, 3); // Imposta la profondità desiderata (es. 3)
        if (bestMove == null || bestMove.Length != 4)
        {
            Debug.Log("Nessuna mossa valida trovata dall'IA.");
            // Gestire la fine del gioco o lo stallo
            return;
        }

        if (bestMove.Length == 4)
        {
            int startX = bestMove[0];
            int startY = bestMove[1];
            int endX = bestMove[2];
            int endY = bestMove[3];

            Debug.Log($"Start=({startX},{startY})  Finish=({endX},{endY})");

            PieceStatus movingPiece = Pieces[startX, startY];

            SelectPiece(movingPiece.gameObject);
            if (Pieces[endX, endY] == null)
            {
                MovePiece(selectedPiece, new Vector2(endX, endY));
            }
            else
            {
                AttackPiece(movingPiece, Pieces[endX, endY]);
            }

            selectedPiece = null;

            currentTurn = Turn.Player;
        }

        CleanTempObjects();
    }

    private void CleanTempObjects()
    {
        foreach (GameObject toDestroy in ai.ToDestroy)
        {
            Destroy(toDestroy);
        }
    }

    void HighlightMoves()
    {

        //int[] coord = GetPositionFromPiece(selectedPiece);
        PieceStatus pieceStatus = selectedPiece.GetComponent<PieceStatus>();

        HashSet<int[]> possibleMoves = cbm.GetPossibleMovesForPiece(pieceStatus, Pieces);
        string possibleMovesStr = string.Join(",", possibleMoves.Select(move => "[" + string.Join(",", move) + "]"));
        Debug.Log("Possible moves: " + possibleMovesStr);

        foreach (int[] move in possibleMoves)
        {
            int x = move[0];
            int y = move[1];
            int moveType = move[2];
            GameObject square = GetSquare(x, y);

            if (square != null)
            {
                Renderer renderer = square.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                    switch (moveType)
                    {
                        case 1:
                            renderer.material.color = Color.green;
                            break;
                        case 2:
                            renderer.material.color = Color.red;
                            break;
                        default:
                            renderer.material.color = Color.white;
                            break;
                    }


                }
            }
            else
            {
                Debug.Log($"No square found at position ({x}, {y}).");
            }
        }
    }


    void HideMoves()
    {
        // Itera su tutte le caselle della scacchiera per nascondere le evidenziazioni
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                GameObject square = GetSquare(x, y);
                if (square != null)
                {
                    // Rende la casella invisibile o ripristina il suo stato iniziale
                    Renderer renderer = square.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.enabled = false;
                    }
                }
            }
        }
    }

    public void SelectPiece(GameObject piece)
    {
        if (piece != null)
        {
            Debug.Log("Pezzo non nullo");
            //Deselect piece if is selected while is still selected
            if (selectedPiece == piece)
            {
                Debug.Log("Deselect piece");
                selectedPiece = null;
            }
            else
            {
                if (currentTurn == Turn.Player)
                {
                    PieceStatus pieceStatus = piece.GetComponent<PieceStatus>();
                    // Check if is attack
                    if (pieceStatus != null && selectedPiece != null && pieceStatus.PieceColor == PieceColor.Black)
                    {
                        HandleSquareClick(squares[(int)pieceStatus.Position.x, (int)pieceStatus.Position.y].GetComponent<BoardSquare>());
                    }
                    else if (pieceStatus.PieceColor == PieceColor.White)
                    {
                        selectedPiece = piece;
                    }
                }
                else
                {
                    selectedPiece = piece;
                }
            }
        }
        else
        {
            Debug.Log("Pezzo nullo");
        }
        HideMoves();
        showMovesFlag = true;
        //highlightedFlag = selectedPiece != null;
    }


    public void SetShowMovesFlag(bool value)
    {
        showMovesFlag = value;
    }

    GameObject GetSquare(int x, int y)
    {
        return GameObject.Find($"Square_{x}_{y}");
    }

    // Metodo per inizializzare il piano di gioco
    void InitializeBoard()
    {
        if (board != null)
        {
            planeTransform = board.transform.Find("Plane");
            if (planeTransform != null)
            {
                // Calcola la dimensione delle caselle in base alle dimensioni del piano
                squareSize = planeTransform.localScale.x * 10 / boardSize; // La scala del piano moltiplicata per 10 poich� il piano standard di Unity � di 10 unit�
            }
            else
            {
                Debug.LogError("Plane non trovato come figlio di Board!");
            }
        }
        else
        {
            Debug.LogError("Board non assegnato!");
        }

        squares = new GameObject[boardSize, boardSize];

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                // Crea un nuovo GameObject piano
                GameObject newSquare = GameObject.CreatePrimitive(PrimitiveType.Plane);

                // Add tag to square
                newSquare.tag = Constants.SQUARE_TAG;

                // Add component to square
                BoardSquare boardSquare = newSquare.AddComponent<BoardSquare>();
                boardSquare.Position = new Vector2(x, y);

                // Imposta la scala del piano in base alla dimensione della casella
                newSquare.transform.localScale = new Vector3(squareSize / 10f, 1f, squareSize / 10f);

                // Calcola la posizione della casella rispetto al piano
                Vector3 squarePosition = new Vector3(
                    x * squareSize + planeTransform.position.x - planeTransform.localScale.x * 5 + squareSize / 2,
                    planeTransform.position.y + 0.0001f,
                    y * squareSize + planeTransform.position.z - planeTransform.localScale.z * 5 + squareSize / 2
                );

                // Posiziona la casella nella posizione corretta
                newSquare.transform.position = squarePosition;

                // Assegna un nome alla casella basato sulla posizione
                newSquare.name = $"Square_{x}_{y}";

                // Rimuovi o disabilita il MeshRenderer per rendere il piano invisibile
                MeshRenderer renderer = newSquare.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = false;
                }

                // Imposta il nuovo piano come figlio del piano principale
                newSquare.transform.parent = board.transform;

                // Salva la casella nell'array per futuri riferimenti
                squares[x, y] = newSquare;
            }
        }

        if (planeTransform != null)
        {
            Destroy(planeTransform.gameObject);
        }
        else
        {
            Debug.LogError("Plane non trovato per la distruzione!");
        }
    }

    PieceStatus[,] LoadBoardFromBoardData()
    {
        if (BoardData == null)
        {
            Debug.Log("BoardData not specified. Cannot create Board");
            return null;
        }
        Riga = BoardData.GridSize.x;
        Colonna = BoardData.GridSize.y;
        PieceStatus[,] result = new PieceStatus[Riga, Colonna];
        for (int i = 0; i < Riga; i++)
        {
            for (int j = 0; j < Colonna; j++)
            {
                GameObject obj = GetPieceFromId(BoardData.GetCell(j, i));
                if (obj != null)
                {
                    obj = Instantiate(obj, GetSquare(i, j).transform.position, GetSquare(i, j).transform.rotation);
                    PieceStatus pieceStatus = obj.GetComponent<PieceStatus>();
                    pieceStatus.Position = new Vector2(i, j);
                    result[i, j] = pieceStatus;
                }
            }
        }
        return result;

    }

    GameObject GetPieceFromId(int id)
    {
        foreach (var prefab in this.PiecePrefabs)
        {
            if (prefab.GetComponent<PieceStatus>().ID == id)
            {
                return prefab;
            }
        }
        return null;
    }

    internal void HandleSquareClick(BoardSquare boardSquare)
    {
        if (currentTurn != Turn.Player || selectedPiece == null) return;

        // Esegue il movimento o l'attacco
        HashSet<int[]> possibleMoves = cbm.GetPossibleMovesForPiece(selectedPiece.GetComponent<PieceStatus>(), Pieces);
        foreach (int[] move in possibleMoves)
        {
            if (move[0] == boardSquare.Position.x && move[1] == boardSquare.Position.y)
            {
                if (move[2] == 1)
                {
                    MovePiece(selectedPiece, boardSquare.Position);
                }
                else if (move[2] == 2)
                {
                    AttackPiece(selectedPiece.GetComponent<PieceStatus>(), Pieces[(int)boardSquare.Position.x, (int)boardSquare.Position.y]);
                }

                selectedPiece = null;
                showMovesFlag = true;

                // Cambia il turno all'IA
                currentTurn = Turn.AI;
                break;
            }
        }
    }


    private void AttackPiece(PieceStatus attacker, PieceStatus target)
    {

        target.TakeDamage(attacker.Attack);
        if (target.Hp <= 0)
        {
            Pieces[(int)target.Position.x, (int)target.Position.y] = attacker;
            Pieces[(int)attacker.Position.x, (int)attacker.Position.y] = null;
            attacker.Position = target.Position;

            // Phisical movement
            attacker.transform.position = squares[(int)target.Position.x, (int)target.Position.y].transform.position;

            // Delete target piece
            Destroy(target.gameObject);

        }
    }

    private void MovePiece(GameObject piece, Vector2 destination)
    {
        if (piece == null || destination == null) return;
        PieceStatus pieceStatus = piece.GetComponent<PieceStatus>();

        // Perform logical movement
        Pieces[(int)pieceStatus.Position.x, (int)pieceStatus.Position.y] = null;
        Pieces[(int)destination.x, (int)destination.y] = pieceStatus;
        pieceStatus.Position = destination;

        // Perform phisical movement
        piece.transform.position = squares[(int)destination.x, (int)destination.y].transform.position;

    }

    public BoardData GetBoardData()
    {
        return new BoardData(currentTurn, Pieces);
    }

    public void BuildFromData(BoardData bData)
    {
        this.currentTurn = bData.currentTurn;

        Riga = bData.piecesData.GetLength(0);
        Colonna = bData.piecesData.GetLength(1);
        PieceStatus[,] result = new PieceStatus[Riga, Colonna];
        for (int i = 0; i < Riga; i++)
        {
            for (int j = 0; j < Colonna; j++)
            {
                GameObject obj = GetPieceFromId(bData.piecesData[i, j].ID);
                if (obj != null)
                {
                    obj = Instantiate(obj, GetSquare(i, j).transform.position, GetSquare(i, j).transform.rotation);
                    PieceStatus pieceStatus = obj.GetComponent<PieceStatus>();
                    pieceStatus.BuildFromData(bData.piecesData[i, j]);
                    result[i, j] = pieceStatus;
                }
            }
        }
        this.Pieces = result;
    }
}
