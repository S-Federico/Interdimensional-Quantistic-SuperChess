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
    private Turn currentTurn = Turn.Player;
    [SerializeField] private GameObject board;
    public Array2DInt BoardData;
    public List<GameObject> PiecePrefabs;
    public PieceStatus[,] Pieces;
    private int Riga;
    private int Colonna;
    private ChessBoardModel cbm;

    private bool showMovesFlag;
    private bool alreadyExcecuting;

    public GameObject selectedPiece;
    private PlayerManager Player;
    public OpponentManager opponent;
    private BoardBehaviour boardBehaviour;
    public GameObject plane_consumables;
    public List<GameObject> consumables = new List<GameObject>();

    void Start()
    {
        cbm = new ChessBoardModel();
        Player = GameObject.FindAnyObjectByType<PlayerManager>();
        boardBehaviour = board.GetComponent<BoardBehaviour>();
        boardBehaviour.InitializeBoard();

        //prima o poi questa cosa sarà fatta con scriptable object prendendo da una serie finita di opponent, 
        //con le loro formazioni, buff ecc e livello, che fa scalare il tutto, oltre a opening lines e musiche
        opponent = GameObject.FindAnyObjectByType<OpponentManager>();
        AssignModifiers();

        LoadConsumables();

        showMovesFlag = false;
        alreadyExcecuting = false;

    }

    void Update()
    {
        GameManager.Instance.IsGameOver = ChessAI.IsGameOver(Pieces);
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
                opponent.ExecuteAITurn(Pieces);
                currentTurn = Turn.Player;
            }
        }

        showMovesFlag = false;
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
        for (int x = 0; x < boardBehaviour.BoardSize; x++)
        {
            for (int y = 0; y < boardBehaviour.BoardSize; y++)
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
                        HandleSquareClick(boardBehaviour.squares[(int)pieceStatus.Position.x, (int)pieceStatus.Position.y].GetComponent<BoardSquare>());
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

    public GameObject GetSquare(int x, int y)
    {
        return boardBehaviour.GetSquare(x, y);
    }



    public PieceStatus[,] LoadBoardFromBoardData()
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
                    PieceStatus pieceStatus = obj.GetComponent<PieceStatus>();
                    // Here get PieceStatus from prefab from inspector. Then build PieceData from it and store in 
                    // PlayerInfo. In this way it can be read from when the game starts
                    if (pieceStatus.PieceColor == PieceColor.White)
                    {
                        GameManager.Instance.GameInfo.PlayerInfo.ExtraPieces.Add(PieceData.FromPieceStatus(pieceStatus));
                    }
                    // If the piece is black, instantiate directly in board
                    else
                    {
                        obj = Instantiate(obj, GetSquare(i, j).transform.position, GetSquare(i, j).transform.rotation);
                        pieceStatus.Position = new Vector2(i, j);
                        result[i, j] = pieceStatus;
                    }

                }
            }
        }
        this.Pieces = result;
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


    public void AttackPiece(PieceStatus attacker, PieceStatus target)
    {
        target.TakeDamage(attacker.Attack);
        if (target.Hp <= 0)
        {
            Pieces[(int)target.Position.x, (int)target.Position.y] = attacker;
            Pieces[(int)attacker.Position.x, (int)attacker.Position.y] = null;
            attacker.Position = target.Position;

            // Phisical movement
            attacker.transform.position = boardBehaviour.squares[(int)target.Position.x, (int)target.Position.y].transform.position;

            // Delete target piece
            Destroy(target.gameObject);

        }
    }

    public void MovePiece(GameObject piece, Vector2 destination)
    {
        if (piece == null || destination == null) return;
        PieceStatus pieceStatus = piece.GetComponent<PieceStatus>();

        // Perform logical movement
        Pieces[(int)pieceStatus.Position.x, (int)pieceStatus.Position.y] = null;
        Pieces[(int)destination.x, (int)destination.y] = pieceStatus;
        pieceStatus.Position = destination;

        // Perform phisical movement
        piece.transform.position = boardBehaviour.squares[(int)destination.x, (int)destination.y].transform.position;

    }

    public BoardData GetBoardData()
    {
        return new BoardData(currentTurn, Pieces);
    }

    public void BuildFromData(BoardData bData)
    {
        if (bData != null)
        {
            this.currentTurn = bData.currentTurn;

            Riga = bData.piecesData.GetLength(0);
            Colonna = bData.piecesData.GetLength(1);
            PieceStatus[,] result = new PieceStatus[Riga, Colonna];
            for (int i = 0; i < Riga; i++)
            {
                for (int j = 0; j < Colonna; j++)
                {
                    if (bData.piecesData[i, j] == null) continue;
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

    public void AssignModifiers()
    {
        foreach (ItemData manual in Player.PManuals)
        {
            if (manual != null)
            {
                ScriptableManual ScriptManual = manual.scriptableItem as ScriptableManual;
                for (int i = 0; i < boardBehaviour.BoardSize; i++)
                {
                    for (int j = 0; j < boardBehaviour.BoardSize; j++)
                    {
                        if (ScriptManual.ApplicationMatrix.GetCell(j, i) == 1)
                        {
                            foreach (ScriptableStatusModifier modi in manual.scriptableItem.Modifiers)
                            {
                                GetSquare(i, j).GetComponent<BoardSquare>().ManualsModifiers.Add(modi);
                            }
                        }
                    }
                }
            }
        }
    }

    public void LoadConsumables()
    {
        // Recuperiamo le dimensioni del piano
        float planeLength = plane_consumables.GetComponent<Renderer>().bounds.size.x;

        // Verifichiamo che lo spazio disponibile sia sufficiente per includere il padding tra i consumables
        float padding = 0.1f;
        int numberOfConsumables = Player.PConsumables.Count;
        Debug.Log("Consumabili da istanziare " + numberOfConsumables);
        float totalRequiredSpace = numberOfConsumables * padding;
        if (planeLength < totalRequiredSpace)
        {
            Debug.LogError("Non c'è abbastanza spazio per posizionare i consumables con il padding richiesto.");
            return;
        }

        // Calcoliamo la distanza tra ogni manuale (incluso il padding) sull'asse X
        float spacing = (planeLength - (padding * (numberOfConsumables - 1))) / (numberOfConsumables + 1);  // +1 per evitare di posizionare manuali fuori dal bordo

        // Recuperiamo la posizione di partenza del piano
        Vector3 planeStartPosition = plane_consumables.transform.position;
        float planeMinX = planeStartPosition.x - planeLength / 2;

        int i = 0;
        foreach (ItemData consumable in Player.PConsumables)
        {
            if (consumable != null)
            {
                ScriptableItem scriptableConsum = consumable.scriptableItem;
                // Calcoliamo la posizione in cui piazzare l'oggetto
                Vector3 position = new Vector3(
                    planeMinX + spacing * (i + 1) + padding * i,  // Posizionamento lungo l'asse X con padding
                    planeStartPosition.y,                        // Stessa altezza Y del piano
                    planeStartPosition.z                         // Stessa posizione Z del piano
                );
                // Creiamo una leggera rotazione casuale sull'asse Y
                Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(70, 100), 0);
                // Istanziamo il manuale selezionato
                GameObject obj = Instantiate(scriptableConsum.Prefab, position, rotation);

                Vector3 scale = new Vector3(20, 20, 20);
                obj.transform.localScale = Vector3.Scale(obj.transform.localScale, scale);

                consumables.Add(obj);
                // Stampa per debug
                Debug.Log("Selezionato manuale: " + scriptableConsum.name + " alla posizione " + position);
            }
            else
            {
                Debug.LogError("Consumabile " + i + " nullo");
            }
            i++;
        }
    }
}
