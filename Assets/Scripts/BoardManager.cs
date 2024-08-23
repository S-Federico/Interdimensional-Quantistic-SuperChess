using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject board;         // Riferimento al GameObject "Board" che contiene il piano
    [SerializeField] private GameObject piecePrefab;   // Prefab del pezzo da instanziare

    private Transform planeTransform;                 // Riferimento al Transform del Piano
    private float squareSize;                         // Dimensione di una singola casella
    private int boardSize = 8;                        // Dimensione della scacchiera (8x8)
    private GameObject[,] squares;                    // Array bidimensionale per memorizzare le caselle

    void Start()
    {
        InitializeBoard();
        GenerateBoard();
        DestroyPlane(); // Distrugge il piano dopo aver generato la scacchiera
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
                squareSize = planeTransform.localScale.x * 10 / boardSize; // La scala del piano moltiplicata per 10 poiché il piano standard di Unity è di 10 unità
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
    }

    // Metodo per generare dinamicamente le caselle della scacchiera
    void GenerateBoard()
    {
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
    }

    // Metodo per distruggere il piano dopo aver generato la scacchiera
    void DestroyPlane()
    {
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
