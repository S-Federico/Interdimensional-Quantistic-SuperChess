using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Array2DEditor;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ItemData : MonoBehaviour, IClickable, IPointerEnterHandler, IPointerExitHandler
{
    public Dictionary<(int, int), int> affectedCells;
    public ScriptableItem scriptableItem;
    public string ScriptableItemPath;
    public bool bought;
    public bool selected;
    public bool used;
    public bool alreadyElevated;
    public float el;

    public GameObject buyTag;
    public GameObject priceTag;
    public GameObject sellTag;
    public GameObject useTag;
    public bool shopScaling;

    //Ancore per i bottoni
    Vector3 bottomLeft;
    Vector3 bottomRight;
    Vector3 bottomCenter;
    public PieceData pieceData = null;

    public int pieceprice = 10;
    BoardManager boardManager = null;
    public bool showCells = false;
    public Tooltip tooltip;
    public void Start()
    {
        tooltip = GameObject.FindObjectOfType<Tooltip>();

        // Calcola il bounding box del modello del GameObject padre (assumendo che il modello abbia un MeshRenderer)
        Renderer modelRenderer = this.gameObject.GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
        {
            float objHeight = modelRenderer.bounds.size.y;
            float objWidth = modelRenderer.bounds.size.x;
            float objLenght = modelRenderer.bounds.size.z;

            bottomCenter = new Vector3();
            bottomCenter.z = this.gameObject.transform.position.z - (objLenght / 2) - 0.06f;
            bottomCenter.y = this.gameObject.transform.position.y;
            bottomCenter.x = this.gameObject.transform.position.x;

            bottomLeft = new Vector3();
            bottomLeft.z = this.gameObject.transform.position.z - (objLenght / 2) - 0.06f;
            bottomLeft.y = this.gameObject.transform.position.y;
            bottomLeft.x = this.gameObject.transform.position.x - 0.07f;

            bottomRight = new Vector3();
            bottomRight.z = this.gameObject.transform.position.z - (objLenght / 2) - 0.06f;
            bottomRight.y = this.gameObject.transform.position.y;
            bottomRight.x = this.gameObject.transform.position.x + 0.07f;

        }
        else
        {
            Debug.LogError("Non è stato trovato un Renderer nel GameObject padre o nei suoi figli.");
        }

        el = 0.1f;
        int buyprice = pieceprice;
        if (scriptableItem != null)
            buyprice = scriptableItem.Price;

        buyTag = Instantiate(Resources.Load<GameObject>("ButtonPrefab"));
        sellTag = Instantiate(Resources.Load<GameObject>("ButtonPrefab"));
        useTag = Instantiate(Resources.Load<GameObject>("ButtonPrefab"));
        priceTag = Instantiate(Resources.Load<GameObject>("ButtonPrefab"));

        SetButton(buyTag, bottomLeft, ButtonType.Buy, "Buy");
        SetButton(sellTag, bottomLeft, ButtonType.Sell, $"Sell {(buyprice / 2) + 1}$");
        SetButton(useTag, bottomRight, ButtonType.Use, "Use");
        SetButton(priceTag, bottomRight, ButtonType.PriceTag, $"{buyprice}$");

        if (GameObject.FindObjectOfType<ShopManager>() == null)
        {
            bought = true;
            boardManager = GameObject.FindObjectOfType<BoardManager>();

        }
        used = false;
        affectedCells = new Dictionary<(int, int), int>();
    }

    public void SetButton(GameObject b, Vector3 anchor, ButtonType type, string text)
    {
        b.transform.position = this.gameObject.transform.position;
        b.transform.position = anchor;

        // Imposta il padre del bottone
        b.transform.SetParent(transform);
        b.SetActive(false);

        //Imposta il testo
        b.GetComponentInChildren<TextMeshProUGUI>().text = text;
        //Imposta il buttontype
        b.GetComponent<ButtonBehaviour>().associatedItem = this;
        b.GetComponent<ButtonBehaviour>().type = type;
    }

    public void Update()
    {
        if (!shopScaling)
        {
            if (pieceData != null)
            {
                SetScale(0.1f);
            }
            else
            {
                SetScale(10f);
                el = 1f;
            }
            shopScaling = true;
        }

        if (selected)
        {
            if (!alreadyElevated)
            {
                alreadyElevated = true;
                ElevateItem();
                ShowTags();
            }
        }
        else
        {
            if (alreadyElevated)
            {
                alreadyElevated = false;
                DeElevateItem();
                HideTags();
                if (FindAnyObjectByType<BoardManager>() != null)
                    FindAnyObjectByType<BoardManager>().selectedConsumable = null;
            }

        }

        if (!selected)
            used = false;

        if (used && boardManager != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                BoardSquare square = hit.collider.GetComponent<BoardSquare>();
                if (square == null)
                {
                    hit.collider.TryGetComponent<PieceStatus>(out PieceStatus piece);
                    if (piece != null && piece.Position.x >= 0)
                    {
                        square = boardManager.GetSquare((int)piece.Position.x, (int)piece.Position.y).GetComponent<BoardSquare>();
                    }
                }
                if (square != null)
                {
                    Vector2 position = square.Position;
                    ScriptableConsumable consumable = scriptableItem as ScriptableConsumable;
                    int[,] applicationMatrix = Utility.ConvertA2DintToIntMatrix(consumable.ApplicationMatrix);

                    // Calcola l'offset centrale per centrare la matrice sulla casella centrale
                    int offsetX = applicationMatrix.GetLength(0) / 2;
                    int offsetY = applicationMatrix.GetLength(1) / 2;

                    for (int i = 0; i < applicationMatrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < applicationMatrix.GetLength(1); j++)
                        {
                            if (applicationMatrix[i, j] == 1)
                            {
                                int relativeX = (int)position.x + i - offsetX;
                                int relativeY = (int)position.y + j - offsetY;

                                if (relativeX >= 0 && relativeX <= 7 && relativeY >= 0 && relativeY <= 7)
                                {
                                    var key = (relativeX, relativeY);

                                    int value = consumable.ConsumableType == ConsumablesType.Cell ? 3 : 4;

                                    boardManager.highlightedSquares[key] = value;

                                }
                            }
                        }
                    }
                }

            }
        }

        if (showCells && scriptableItem is ScriptableManual && boardManager!=null)
        {
            ScriptableManual manual = scriptableItem as ScriptableManual;
            int[,] applicationMatrix = Utility.ConvertA2DintToIntMatrix(manual.ApplicationMatrix);

            for (int i = 0; i < applicationMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < applicationMatrix.GetLength(1); j++)
                {
                    if (applicationMatrix[i, j] == 1)
                    {
                        var key = (i, j);
                        int value = 5;
                        boardManager.highlightedSquares[key] = value;
                    }
                }
            }
        }
    }

    public void SetScale(float scaleFactor)
    {
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        transform.localRotation = Quaternion.Euler(0, -90, 0);
    }

    public void DeElevateItem()
    {
        Vector3 p = this.gameObject.transform.position;
        Vector3 newP = new Vector3(p.x, p.y - el, p.z);
        this.gameObject.transform.position = newP;
        Debug.Log($"Deelevated to {newP.y}");
    }

    public void HideTags()
    {
        Debug.Log($"Price hidden");
        buyTag.SetActive(false);
        sellTag.SetActive(false);
        useTag.SetActive(false);
        priceTag.SetActive(false);
    }

    public void ElevateItem()
    {
        Vector3 p = this.gameObject.transform.position;
        Vector3 newP = new Vector3(p.x, p.y + el, p.z);
        this.gameObject.transform.position = newP;
        Debug.Log($"Elevated to {newP.y}");
    }

    public void ShowTags()
    {
        if (bought)
        {
            sellTag.SetActive(true);
            useTag.SetActive(true);
        }
        else
        {
            buyTag.SetActive(true);
            priceTag.SetActive(true);
        }
    }

    public void OnClick()
    {
        ToggleSelected();
    }

    public void ToggleSelected()
    {
        selected = !selected;
    }

    public void OnButtonClicked(ButtonType buttonType)
    {
        switch (buttonType)
        {
            case ButtonType.Buy:
                ToggleSelected();
                ShopManager shop = FindAnyObjectByType<ShopManager>();
                if (shop != null)
                {
                    shop.BuyItem(this);
                }
                break;
            case ButtonType.Sell:
                ToggleSelected();
                PlayerManager player = GameObject.Find("Player").GetComponent<PlayerManager>();
                if (player != null)
                {
                    GameManager.Instance.GameInfo.PlayerInfo.Money += (scriptableItem.Price / 2) + 1;
                    player.RemoveItem(this);
                    Destroy(this.gameObject);
                }
                else
                {
                    Debug.Log("Player null!");
                }
                break;
            case ButtonType.Use:
                FindAnyObjectByType<BoardManager>().selectedConsumable = this;
                used = true;
                break;
            case ButtonType.PriceTag:
                ToggleSelected();
                break;
            default:
                ToggleSelected();
                break;
        }
    }

    public void UseItem(BoardSquare boardSquare)
    {
        Debug.Log($"Item {scriptableItem.Name} used!");

        BoardManager board = FindAnyObjectByType<BoardManager>();

        ScriptableConsumable ScriptCons = this.scriptableItem as ScriptableConsumable;

        List<Vector2Int> affectedCells = CheeseOfThruth(boardSquare, board.Pieces, ScriptCons);

        switch (ScriptCons.ConsumableType)
        {
            case ConsumablesType.Piece:
                bool empty = true;
                foreach (Vector2Int cell in affectedCells) if (board.Pieces[cell.x, cell.y] != null) empty = false;

                if (empty)
                {
                    HideTags();
                    FindAnyObjectByType<BoardManager>().selectedConsumable = null;
                    ToggleSelected();
                    return;
                }

                foreach (Vector2Int cell in affectedCells)
                {
                    PieceStatus piece = board.Pieces[cell.x, cell.y];
                    if (piece != null)
                    {
                        foreach (ScriptableStatusModifier modi in ScriptCons.Modifiers)
                        {
                            piece.appliedModifiers.Add(ScriptableModifierData.FromScriptableObject(modi));
                        }
                    }
                }
                break;

            case ConsumablesType.Cell:
                foreach (Vector2Int cell in affectedCells)
                {
                    foreach (ScriptableStatusModifier modi in ScriptCons.Modifiers)
                    {
                        board.GetSquare(cell.x, cell.y).GetComponent<BoardSquare>().ManualsModifiers.Add(modi);
                    }
                }
                break;

            default:
                HideTags();
                FindAnyObjectByType<BoardManager>().selectedConsumable = null;
                ToggleSelected();
                return;
        }

        Done();
    }

    public void Done()
    {
        PlayerManager player = GameObject.Find("Player").GetComponent<PlayerManager>();
        player.RemoveItem(this);
        Destroy(this.gameObject);
    }

    public List<Vector2Int> CheeseOfThruth(BoardSquare boardSquare, PieceStatus[,] board, ScriptableConsumable consumable)
    {
        int riga = (int)boardSquare.Position.x;
        int colonna = (int)boardSquare.Position.y;
        int[,] matrix = ConvertArray2D(consumable.ApplicationMatrix);
        int offsetRighe = matrix.GetLength(0) / 2;
        int offsetColonne = matrix.GetLength(1) / 2;

        List<Vector2Int> result = new List<Vector2Int>();

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                int newRiga = riga + (i - offsetRighe);
                int newColonna = colonna + (j - offsetColonne);

                // Controlla che la nuova posizione sia all'interno della scacchiera
                if (newRiga >= 0 && newRiga < board.GetLength(0) && newColonna >= 0 && newColonna < board.GetLength(1))
                {
                    if (matrix[i, j] == 1)
                    {
                        result.Add(new Vector2Int(newRiga, newColonna));
                    }
                }
            }
        }
        return result;
    }

    public int[,] ConvertArray2D(Array2DInt matrix)
    {
        // Crea una nuova matrice int[,] con le stesse dimensioni di Array2DInt
        int[,] result = new int[matrix.GridSize.x, matrix.GridSize.y];

        // Itera su tutte le celle e copia i valori
        for (int i = 0; i < matrix.GridSize.x; i++)
        {
            for (int j = 0; j < matrix.GridSize.y; j++)
            {
                result[i, j] = matrix.GetCell(i, j);
            }
        }

        return result;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            if (scriptableItem != null)
            {
                tooltip.SetText(scriptableItem.Name, scriptableItem.Description);
            }
            else
                tooltip.SetText("Pezzo", "OnePiece");
            tooltip.ShowAfterDelay();
        }
        showCells = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            tooltip.Hide();
        }
        showCells = false;
    }
}


