using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject board;         // Riferimento al GameObject "Board" che contiene il piano
    [SerializeField] private GameObject piecePrefab;   // Prefab del pezzo da instanziare
    [SerializeField] private int riga;
    [SerializeField] private int colonna;

    private Transform planeTransform;                 // Riferimento al Transform del Piano
    private float squareSize;                         // Dimensione di una singola casella
    private int boardSize = 8;                        // Dimensione della scacchiera (8x8)
    private GameObject[,] squares;                    // Array bidimensionale per memorizzare le caselle

    private ChessBoardModel cbm;

    //flag da cancellare
    private bool showMovesFlag;
    private bool highlightedFlag;


    void Start()
    {
        cbm = new ChessBoardModel(8, 8);
        InitializeBoard();

        showMovesFlag = false;

        //piazza il pezzo su una casella
        Instantiate(piecePrefab, GetSquare(riga, colonna).transform.position, GetSquare(riga, colonna).transform.rotation);

        int[,] matrice = new int[17, 17]
        {
            {1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1},
            {0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0},
            {0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0},
            {0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0},
            {0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0},
            {0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0},
            {0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0},
            {0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0},
            {1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1}
        };
        Instantiate(piecePrefab, GetSquare(riga-1, colonna).transform.position, GetSquare(riga-1, colonna).transform.rotation);

        cbm.PlacePiece(new Piece(PieceType.Queen, 1, 1, PieceColor.White, matrice, new int[] { riga, colonna }));
        cbm.PlacePiece(new Piece(PieceType.Queen, 1, 1, PieceColor.White, matrice, new int[] { riga-1, colonna }));

    }

    void Update()
    {

        //tenere una variabile selected chessman, se non è null mostriamo le mosse

        if (showMovesFlag)
        {   
            if(!highlightedFlag){
                HighlightMoves();
                highlightedFlag=true;
            }
        }
        else
        {   
            if(highlightedFlag){
                HideMoves();
                highlightedFlag=false;
            }
        }

    }

    void HighlightMoves()
    {
        HashSet<int[]> possibleMoves = cbm.GetPossibleMovesForPiece(riga, colonna);

        Debug.Log($"Found {possibleMoves.Count} possible moves.");

        foreach (int[] move in possibleMoves)
        {
            int x = move[0];
            int y = move[1];
            GameObject square = GetSquare(x, y);

            if (square != null)
            {
                Debug.Log($"Highlighting square at position ({x}, {y}).");
                Renderer renderer = square.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.enabled = true;
                    renderer.material.color = Color.green;
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


    public void ToggleShowMovesFlag()
    {
        showMovesFlag = !showMovesFlag;
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

}
