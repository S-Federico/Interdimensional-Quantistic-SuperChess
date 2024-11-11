using System.Collections.Generic;
using Array2DEditor;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;


public class BoardManager : MonoBehaviour
{
    public Dictionary<(int, int), int> highlightedSquares;
    public Turn currentTurn = Turn.Player;
    [SerializeField] private GameObject board;
    public Array2DInt BoardData;
    public List<GameObject> PiecePrefabs;
    public PieceStatus[,] Pieces = new PieceStatus[8, 8];
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
    private GameObject plane_manuals;
    public GameObject playerPiecesPlane;
    public GameObject opponentPiecesPlane;

    public List<GameObject> consumables;
    public ItemData selectedConsumable;

    [SerializeField] private bool isTutorial = false;
    public bool IsTutorial { get => isTutorial; }

    private GameInfo gameInfo;

    void Start()
    {
        gameInfo = GameManager.Instance.GameInfo;
        cbm = new ChessBoardModel();
        Player = GameObject.FindAnyObjectByType<PlayerManager>();
        boardBehaviour = board.GetComponent<BoardBehaviour>();
        boardBehaviour.InitializeBoard();
        highlightedSquares = new Dictionary<(int, int), int>();
        opponent = GameObject.FindAnyObjectByType<OpponentManager>();

        AssignModifiers();

        LoadConsumables();

        plane_manuals = GameObject.Find("ManualPlane");
        LoadManuals();
        showMovesFlag = false;
        alreadyExcecuting = false;

    }

    void Update()
    {
        // Check on GameManager.Instance.IsGameover to avoit invoking GameOver function every frame
        if (Pieces != null && ChessAI.IsGameOver(Pieces) && !GameManager.Instance.IsGameOver && !isTutorial)
        {
            PieceColor? winner = ChessAI.GetWinner(Pieces);
            GameManager.Instance.GameOver(winner);
        }

        if (currentTurn == Turn.Player)
        {
            alreadyExcecuting = false;
            foreach (PieceStatus p in Player.pieces)
            {
                if (p != null && p.gameObject.TryGetComponent<DraggableBehaviour>(out DraggableBehaviour draggableBehaviour))
                {
                    draggableBehaviour.isDraggable = true;
                }
            }
        }
        else if (currentTurn == Turn.AI)
        {
            if (!alreadyExcecuting)
            {
                alreadyExcecuting = true;

                if (!isTutorial)
                    opponent.ExecuteAITurn(Pieces);

                currentTurn = Turn.Player;
            }
        }
    }

    void LateUpdate()
    {
        UpdateHighlightedSquares();
        highlightedSquares.Clear();

    }

    public void UpdateHighlightedSquares()
    {
        if (showMovesFlag && selectedPiece != null && selectedPiece.GetComponent<PieceStatus>().Position != new Vector2(-1f, -1f))
        {
            PieceStatus pieceStatus = selectedPiece.GetComponent<PieceStatus>();
            HashSet<int[]> possibleMoves = cbm.GetPossibleMovesForPiece(pieceStatus, Pieces);

            foreach (int[] move in possibleMoves)
            {
                if (move.Length >= 3)
                {
                    (int, int) key = (move[0], move[1]);
                    int value = move[2];

                    highlightedSquares[key] = value;
                }
            }
        }

        foreach (GameObject square in boardBehaviour.squares)
        {
            square.GetComponent<BoardSquare>().Highlight();
        }
    }

    public void HideMoves()
    {
        highlightedSquares.Clear();
    }


    public void SelectPiece(GameObject piece)
    {
        if (piece != null)
        {
            //Deselect piece if is selected while is still selected
            if (selectedPiece == piece)
            {
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
    }

    public GameObject GetSquare(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return playerPiecesPlane;
        }
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
                        if (pieceStatus.PieceType == PieceType.King)
                        {
                            obj = Instantiate(obj, GetSquare(i, j).transform.position, GetSquare(i, j).transform.rotation);
                            pieceStatus = obj.GetComponent<PieceStatus>();
                            pieceStatus.Position = new Vector2(i, j);
                            result[i, j] = pieceStatus;
                        }
                        else
                        {
                            GameManager.Instance.GameInfo.PlayerInfo.ExtraPieces.Add(PieceData.FromPieceStatus(pieceStatus));

                        }
                    }
                    // If the piece is black, instantiate directly in board
                    else
                    {
                        if (pieceStatus.PieceType == PieceType.King)
                        {
                            obj = Instantiate(obj, GetSquare(i, j).transform.position, GetSquare(i, j).transform.rotation);
                            pieceStatus = obj.GetComponent<PieceStatus>();
                            pieceStatus.Position = new Vector2(i, j);
                            result[i, j] = pieceStatus;
                        }
                        else
                        {
                            GameManager.Instance.GameInfo.OpponentInfo.ExtraPieces.Add(PieceData.FromPieceStatus(pieceStatus));
                        }
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
            if (prefab.GetComponent<PieceStatus>().PrefabID == id)
            {
                return prefab;
            }
        }
        return null;
    }

    internal void HandleSquareClick(BoardSquare boardSquare)
    {
        if (selectedConsumable != null)
        {
            selectedConsumable.UseItem(boardSquare);
            selectedConsumable = null;
            return;
        }

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
        if (piece.GetComponent<PieceStatus>().Position.x != -1)
            Pieces[(int)pieceStatus.Position.x, (int)pieceStatus.Position.y] = null;

        Pieces[(int)destination.x, (int)destination.y] = pieceStatus;

        pieceStatus.Position = destination;

        Vector3 newp = boardBehaviour.squares[(int)destination.x, (int)destination.y].transform.position;


        // Perform phisical movement
        piece.transform.position = boardBehaviour.GetSquare((int)destination.x, (int)destination.y).gameObject.transform.position;

        // Play sound
        SoundManager.Instance.PlaySoundOneShot(Sound.PIECE_MOVE);
    }

    public bool CanPlacePiece(PieceStatus piece)
    {
        if (piece == null)
        {
            return false;
        }
        BoardSquare placement = piece.GetSquareBelow();
        if (placement != null)
        {
            if (cbm.GetAllowedPlacements(piece, Pieces, this).Contains(placement))
            {
                if ((piece.PieceColor == PieceColor.Black && currentTurn == Turn.AI) || (piece.PieceColor == PieceColor.White && currentTurn == Turn.Player))
                    return true;
            }
        }
        return false;
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
                    GameObject obj = GetPieceFromId(bData.piecesData[i, j].PrefabID);
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
            if (!IsTutorial)
                InitializePiecesPlanes(false);
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
        if (numberOfConsumables < 1) return;

        float totalRequiredSpace = numberOfConsumables * padding;
        if (planeLength < totalRequiredSpace)
        {
            Debug.LogError("Non c'è abbastanza spazio per posizionare i consumables con il padding richiesto.");
            return;
        }

        // Calcoliamo la distanza tra ogni consumable (incluso il padding) sull'asse X
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
                // Istanziamo il consumable selezionato
                Quaternion rotation = Quaternion.Euler(0, 0, 0);
                GameObject obj = Instantiate(scriptableConsum.Prefab, position, rotation);
                obj.GetComponent<ItemData>().shopScaling = false;
                consumables.Add(obj);

                // Stampa per debug
                Debug.Log("Selezionato consumable: " + scriptableConsum.name + " alla posizione " + position);
            }
            else
            {
                Debug.LogError("Consumabile " + i + " nullo");
            }
            i++;
        }
    }

    public void LoadManuals()
    {
        // Recuperiamo le dimensioni del piano
        float planeLength = plane_manuals.GetComponent<Renderer>().bounds.size.x;

        // Verifichiamo che lo spazio disponibile sia sufficiente per includere il padding tra i consumables
        float padding = 0.01f;
        int numberOfManuals = Player.PManuals.Count;
        Debug.Log("Manuali da istanziare " + numberOfManuals);
        if (numberOfManuals < 1) return;

        float totalRequiredSpace = numberOfManuals * padding;
        if (planeLength < totalRequiredSpace)
        {
            Debug.LogError("Non c'è abbastanza spazio per posizionare i manuali con il padding richiesto.");
            return;
        }

        // Calcoliamo la distanza tra ogni consumable (incluso il padding) sull'asse X
        float spacing = (planeLength - (padding * (numberOfManuals - 1))) / (numberOfManuals + 1);  // +1 per evitare di posizionare manuali fuori dal bordo

        // Recuperiamo la posizione di partenza del piano
        Vector3 planeStartPosition = plane_manuals.transform.position;
        float planeMinX = planeStartPosition.x - planeLength / 2;

        int i = 0;
        foreach (ItemData manual in Player.PManuals)
        {
            if (manual != null)
            {
                ScriptableItem scriptableMan = manual.scriptableItem;
                // Calcoliamo la posizione in cui piazzare l'oggetto
                Vector3 position = new Vector3(
                    planeMinX + spacing * (i + 1) + padding * i,  // Posizionamento lungo l'asse X con padding
                    planeStartPosition.y,                        // Stessa altezza Y del piano
                    planeStartPosition.z                         // Stessa posizione Z del piano
                );
                // Istanziamo il consumable selezionato
                Quaternion rotation = Quaternion.Euler(0, 0, 0);
                GameObject obj = Instantiate(scriptableMan.Prefab, position, rotation);
                obj.GetComponent<ItemData>().shopScaling = false;

                // Stampa per debug
                Debug.Log("Selezionato manual: " + scriptableMan.name + " alla posizione " + position);
            }
            else
            {
                Debug.LogError("Manuale " + i + " nullo");
            }
            i++;
        }
    }

    public static void MovePiecesFromInventoryToPlanes(GameInfo gameInfo, int npieces)
    {
        // "Equipaggio" i pezzi dall'inventario globale del player e opponent per metterli sul piano
        gameInfo.PlayerInfo.CurrentlyUsedExtraPieces = Utility.SelectCurrentMatchPieces(npieces, gameInfo.PlayerInfo.ExtraPieces);
        gameInfo.OpponentInfo.CurrentlyUsedExtraPieces = Utility.SelectCurrentMatchPieces(npieces, gameInfo.OpponentInfo.ExtraPieces);

        // Rimuovo i pezzi "equipaggiati" dall'inventario globale di player e nemico
        gameInfo.PlayerInfo.ExtraPieces.RemoveAll(p => gameInfo.PlayerInfo.CurrentlyUsedExtraPieces.Contains(p));
        gameInfo.OpponentInfo.ExtraPieces.RemoveAll(p => gameInfo.OpponentInfo.CurrentlyUsedExtraPieces.Contains(p));
    }

    public void InitializePiecesPlanes(bool isNewGame)
    {

        int npieces = 10;

        // Seleziona i pezzi del giocatore e dell'avversario
        // Prima di prenderli dalla pool globale, vede se si tratta di una nuova partita.
        // Se non è nuova partita, allora li prende dal salvataggio
        if (isNewGame)
        {
            MovePiecesFromInventoryToPlanes(gameInfo, npieces);
        }
        List<PieceData> playerPieces = gameInfo.PlayerInfo.CurrentlyUsedExtraPieces;
        List<PieceData> opponentPieces = gameInfo.OpponentInfo.CurrentlyUsedExtraPieces;

        List<PieceStatus> actualOpponentPieces = new List<PieceStatus>();
        List<PieceStatus> actualPlayerPieces = new List<PieceStatus>();

        // Ottieni le dimensioni dei piani
        Vector3 playerPlaneSize = playerPiecesPlane.GetComponent<Renderer>().bounds.size;
        Vector3 opponentPlaneSize = opponentPiecesPlane.GetComponent<Renderer>().bounds.size;

        float padding = playerPlaneSize.z / 12;
        float playerPieceSpacingz = (playerPlaneSize.z - padding) / (npieces / 2);
        float opponentPieceSpacingz = (opponentPlaneSize.z - padding) / (npieces / 2);

        float pieceY = playerPiecesPlane.transform.position.y; // Assume che i piani siano su un'asse Y fissa
        float xoffset = playerPlaneSize.x / 4;

        for (int i = 0; i < playerPieces.Count; i++)
        {
            float pieceX;
            float pieceZ;
            PieceData p = playerPieces.ElementAt(i);
            if (i < (playerPieces.Count / 2))
            {
                pieceZ = (playerPieceSpacingz / 2) + (i * playerPieceSpacingz) + playerPiecesPlane.transform.position.z - (playerPlaneSize.z / 2) + (padding / 2);
                pieceX = playerPiecesPlane.transform.position.x + xoffset;
            }
            else
            {
                pieceZ = (playerPieceSpacingz / 2) + ((i - (playerPieces.Count / 2)) * playerPieceSpacingz) + playerPiecesPlane.transform.position.z - (playerPlaneSize.z / 2) + (padding / 2);
                pieceX = playerPiecesPlane.transform.position.x - xoffset;
            }
            if (p != null)
            {
                GameObject obj = GetPieceFromId(p.PrefabID);
                if (obj != null)
                {
                    obj = Instantiate(obj, new Vector3(pieceX, pieceY, pieceZ), Quaternion.identity);
                    PieceStatus pieceStatus = obj.GetComponent<PieceStatus>();
                    pieceStatus.BuildFromData(p);
                    pieceStatus.Position = new Vector2(-1f, -1f);
                    actualPlayerPieces.Add(pieceStatus);
                }
            }
        }

        pieceY = opponentPiecesPlane.transform.position.y; // Assume che i piani siano su un'asse Y fissa

        for (int i = 0; i < opponentPieces.Count; i++)
        {
            PieceData pd = opponentPieces.ElementAt(i);
            PieceStatus p;

            float pieceX;
            float pieceZ;
            if (i < (opponentPieces.Count / 2))
            {
                pieceX = opponentPiecesPlane.transform.position.x + xoffset;
                pieceZ = (opponentPieceSpacingz / 2) + (i * opponentPieceSpacingz) + opponentPiecesPlane.transform.position.z - (opponentPlaneSize.z / 2) + (padding / 2);
            }
            else
            {
                pieceX = opponentPiecesPlane.transform.position.x - xoffset;
                pieceZ = (opponentPieceSpacingz / 2) + ((i - (opponentPieces.Count / 2)) * opponentPieceSpacingz) + opponentPiecesPlane.transform.position.z - (opponentPlaneSize.z / 2) + (padding / 2);
            }
            GameObject obj = GetPieceFromId(pd.PrefabID);

            if (obj != null)
            {
                obj = Instantiate(obj, new Vector3(pieceX, pieceY, pieceZ), Quaternion.identity);
                p = obj.GetComponent<PieceStatus>();
                p.Position = new Vector2(-1f, -1f);
                actualOpponentPieces.Add(p);
            }
        }

        Player.pieces = actualPlayerPieces;
        opponent.pieces = actualOpponentPieces;

    }

    public void PlayerPiecePositioned(PieceStatus piece)
    {

        Pieces[(int)piece.Position.x, (int)piece.Position.y] = piece;
        Player.pieces.Remove(piece);

        currentTurn = Turn.AI;
        foreach (PieceStatus p in Player.pieces)
        {
            if (p != null && p.gameObject.TryGetComponent<DraggableBehaviour>(out DraggableBehaviour draggableBehaviour))
            {
                draggableBehaviour.isDraggable = false;
            }
        }
    }

}
