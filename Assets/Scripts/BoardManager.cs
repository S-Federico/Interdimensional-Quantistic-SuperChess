using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject board;         // Riferimento al GameObject "Board" che contiene il piano
    [SerializeField] private GameObject piecePrefab;   // Prefab del pezzo da instanziare
    public Array2DInt BoardData;
    public List<GameObject> PiecePrefabs;
    private GameObject[,] Pieces;
    private int Riga;
    private int Colonna;

    private Transform planeTransform;                 // Riferimento al Transform del Piano
    private float squareSize;                         // Dimensione di una singola casella
    private int boardSize = 8;                        // Dimensione della scacchiera (8x8)
    private GameObject[,] squares;                    // Array bidimensionale per memorizzare le caselle

    private ChessBoardModel cbm;

    private bool showMovesFlag;
    private bool highlightedFlag;

    public GameObject selectedPiece;

    void Start()
    {
        cbm = new ChessBoardModel(8, 8);
        cbm = new ChessBoardModel(8, 8);
        InitializeBoard();

        showMovesFlag = false;

        // Questa riga di codice carica i pezzi da inspector
        this.Pieces = LoadBoardFromBoardData();

        // //piazza il pezzo su una casella
        // // Instantiate(piecePrefab,GetSquare(Riga,Colonna).transform.position,GetSquare(Riga,Colonna).transform.rotation);

        // int[,] matrice = new int[17, 17]
        // {
        //     {1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1},
        //     {0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0},
        //     {0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0},
        //     {0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0},
        //     {0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0},
        //     {0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0},
        //     {0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0},
        //     {0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
        //     {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        //     {0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0},
        //     {0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0},
        //     {0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0},
        //     {0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0},
        //     {0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0},
        //     {0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0},
        //     {0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0},
        //     {1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1}
        // };

        // PlacePiece(piecePrefab, 5, 5, PieceType.Queen, PieceColor.White, matrice);
        // PlacePiece(piecePrefab, 5, 3, PieceType.Queen, PieceColor.Black, matrice);
        // PlacePiece(piecePrefab, 3, 5, PieceType.Queen, PieceColor.White, matrice);

        // cbm.PlacePiece(new Piece(PieceType.Queen,1,1,PieceColor.White,matrice,new int[] {Riga,Colonna}));
    }

    void Update()
    {

        if (showMovesFlag)
        {
            if (!highlightedFlag)
            {
                HighlightMoves();
                highlightedFlag = true;
            }
        }
        else
        {
            if (highlightedFlag)
            {
                HideMoves();
                highlightedFlag = false;
            }
        }

    }

    void HighlightMoves()
    {

        int[] coord = GetPositionFromPiece(selectedPiece);

        HashSet<int[]> possibleMoves = cbm.GetPossibleMovesForPiece(coord[0], coord[1]);

        foreach (int[] move in possibleMoves)
        {
            int x = move[0];
            int y = move[1];
            int moveType = move[2];
            Debug.Log("Found move (" + x + "," + y + ")" + "  type:" + moveType);
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

    public void PlacePiece(GameObject piece, int riga, int colonna, PieceType type, PieceColor color, int[,] matrice)
    {
        //questo metodo serve per piazzare i pezzi sulla scacchiera e sul modello
        //Siccome per ora ancora non siamo fissi sul modello dati e sulle interazioni, ci sono pochi controlli
        //nel caso ideale si dovrebbe controllare se sul modello si può piazzare il pezzo e poi nel caso farlo piazzare 
        //inoltre tocca capire come mettere una classe che contiene le informazioni del pezzo o attaccare una data class al prefab o qualcosa Gabri aiuto

        //so che dovrei mettere lo square e il pezzo figli ad un gameobject vuoto comune e sarebbe meglio, ma per qualche motivo non riesco a farlo senza deformare tutto
        GameObject newpiece = Instantiate(piece, GetSquare(riga, colonna).transform.position, GetSquare(riga, colonna).transform.rotation);
        newpiece.transform.localScale = new Vector3(1, 1, 1);
        cbm.PlacePiece(new Piece(type, 1, 1, color, matrice, new int[] { riga, colonna }));

    }

    public void SelectPiece(GameObject piece)
    {
        if (piece != null)
        {
            Debug.Log("Pezzo non nullo");
            //Deselect piece if is selected while is still selected
            if (selectedPiece == piece) 
            { 
                selectedPiece = null;
            }
            else
            {
                selectedPiece = piece;
            }
        }
        else
        {
            Debug.Log("Pezzo nullo");
        }
        showMovesFlag = selectedPiece != null;
        highlightedFlag = selectedPiece != null;
    }


    public void SetShowMovesFlag(bool value)
    {
        showMovesFlag = value;
    }

    GameObject GetSquare(int x, int y)
    {
        return GameObject.Find($"Square_{x}_{y}");
    }

    GameObject GetPiece(int riga, int colonna)
    {
        GameObject piece = null;
        GameObject square = GetSquare(riga, colonna);
        if (square == null)
        {
            piece = square.transform.GetChild(0).gameObject;
        }
        return piece;
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

    GameObject[,] LoadBoardFromBoardData()
    {
        if (BoardData == null)
        {
            Debug.Log("BoardData not specified. Cannot create Board");
            return null;
        }
        Riga = BoardData.GridSize.x;
        Colonna = BoardData.GridSize.y;
        GameObject[,] result = new GameObject[Riga, Colonna];
        for (int i = 0; i < Riga; i++)
        {
            for (int j = 0; j < Colonna; j++)
            {
                GameObject obj = GetPieceFromId(BoardData.GetCell(i, j));
                if (obj != null)
                {
                    result[i, j] = obj;
                    PieceStatus pieceStatus = obj.GetComponent<PieceStatus>();
                    pieceStatus.Position = new Vector2(i, j);
                    Instantiate(obj, GetSquare(i, j).transform.position, GetSquare(i, j).transform.rotation);
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


    public int[] GetPositionFromSquare(GameObject square)
    {
        int[] position = new int[] { -1, -1 };

        if (square != null)
        {
            // Ottieni il nome dell'oggetto square
            string squareName = square.name;

            // Assicurati che il nome inizi con "Square_"
            if (squareName.StartsWith("Square_"))
            {
                // Estrai la parte successiva al prefisso "Square_"
                string[] parts = squareName.Substring(7).Split('_');

                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                    {
                        // Se i numeri sono validi, li assegnamo a position
                        position[0] = x;
                        position[1] = y;
                    }
                    else
                    {
                        Debug.LogError("Il nome del quadrato non contiene numeri validi.");
                    }
                }
                else
                {
                    Debug.LogError("Il nome del quadrato non è nel formato corretto.");
                }
            }
            else
            {
                Debug.LogError("Il nome dell'oggetto non inizia con 'Square_'.");
            }
        }
        else
        {
            Debug.LogError("Square è null!");
        }

        return position;
    }

    public int[] GetPositionFromPiece(GameObject piece)
    {
        int[] position = new int[] { -1, -1 };

        // Ottenere la posizione di partenza del raggio dal GameObject "piece"
        Vector3 rayOrigin = piece.transform.position;
        rayOrigin.y += 1; // Dove 'offsetY' è il valore che vuoi aggiungere

        // Direzione del raggio verso il basso
        Vector3 rayDirection = Vector3.down;

        // Distanza massima che il raggio può percorrere
        float maxDistance = 100f;

        // Raycast per colpire un oggetto nel percorso del raggio
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, maxDistance))
        {
            // Controllo se l'oggetto colpito ha un nome nel formato "Square_x_y"
            string hitObjectName = hit.collider.gameObject.name;
            if (hitObjectName.StartsWith("Square_"))
            {
                // Splitta il nome per estrarre le coordinate x e y
                string[] nameParts = hitObjectName.Split('_');
                if (nameParts.Length == 3)
                {
                    // Prova a convertire le coordinate in numeri interi
                    if (int.TryParse(nameParts[1], out int x) && int.TryParse(nameParts[2], out int y))
                    {
                        position[0] = x;
                        position[1] = y;
                    }
                }
            }
        }
        Debug.Log("colpito: " + position[0] + " " + position[1]);
        return position;
    }


}
