using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject plane_manuals;
    public GameObject plane_pieces;
    public GameObject plane_consumable;
    public Dictionary<string, ScriptableManual> scriptableManualDict = new Dictionary<string, ScriptableManual>();
    public Dictionary<string, ScriptableConsumable> scriptableConsumableDict = new Dictionary<string, ScriptableConsumable>();
    public List<GameObject> manuals = new List<GameObject>();
    public List<GameObject> consumables = new List<GameObject>();
    public List<GameObject> pieces = new List<GameObject>();

    private PlayerManager player;
    private GameObject plane_inventory;
    private List<ItemData> inventory_items = new List<ItemData>();

    int numPieces = 2;

    void Start()
    {
        Debug.Log("SONO LO SHOP MANAGER");
        // Load scriptable objects into dictionaries
        LoadScriptableObjects<ScriptableManual>("ScriptableObjects/Manuals", scriptableManualDict);
        LoadScriptableObjects<ScriptableConsumable>("ScriptableObjects/Consumables", scriptableConsumableDict);

        // Select random items from the dictionaries
        SelectRandomManuals(scriptableManualDict.Count, 0.1f);
        SelectRandomConsumables(scriptableConsumableDict.Count, 0.1f);
        SelectRandomPieces(numPieces, 0.1f);
        player = GameObject.Find("Player").GetComponent<PlayerManager>();
        plane_inventory = GameObject.Find("PlayerInventory");

    }

    public void BuyItem(ItemData item)
    {
        //crea caso speciale per buyItem
        //Quando si compra si genera il PieceData e si aggiunge ai playerextrapieces
        if (item != null)
        {
            if (CanBeBought(item))
            {
                if (item.scriptableItem != null)
                    GameManager.Instance.GameInfo.PlayerInfo.Money -= item.scriptableItem.Price;
                else
                    GameManager.Instance.GameInfo.PlayerInfo.Money -= item.pieceprice;

                //istanzia nell'inventario del player
                if (item.pieceData != null)
                {
                    GameManager.Instance.GameInfo.PlayerInfo.ExtraPieces.Add(item.pieceData);
                }
                else
                {
                    if (item.scriptableItem is ScriptableConsumable)
                    {
                        GameManager.Instance.GameInfo.PlayerInfo.Consumables.Add(item.ScriptableItemPath);
                    }
                    else if (item.scriptableItem is ScriptableManual)
                    {
                        GameManager.Instance.GameInfo.PlayerInfo.Manuals.Add(item.ScriptableItemPath);
                    }
                }
                item.bought = true;
                MoveToInventoryPlane(item);
            }
            else
            {
                Debug.Log("Item non comprato.");
            }
        }
    }

    private void MoveToInventoryPlane(ItemData item)
    {
        inventory_items.Add(item);
        float padding = 0.05f;

        float planeSpace = plane_inventory.GetComponent<Renderer>().bounds.size.z;
        float totalRequiredSpace = inventory_items.Count * padding;

        if (planeSpace < totalRequiredSpace)
        {
            Debug.LogError("Non c'è abbastanza spazio nell'inventario (plane).");
            return;
        }

        float spacing = (planeSpace - (padding * (inventory_items.Count - 1))) / (inventory_items.Count + 1);
        Vector3 planeStartPosition = plane_inventory.transform.position;
        float planeMinZ = planeStartPosition.z - planeSpace / 2;

        for (int i = 0; i < inventory_items.Count; i++)
        {
            Vector3 newPosition = new Vector3(
                planeStartPosition.x,
                planeStartPosition.y,
                planeMinZ + spacing * (i + 1) + padding * i
            );

            //Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(70, 100), 0);
            item.gameObject.transform.position = newPosition;
        }
    }

    public bool CanBeBought(ItemData item)
    {
        if (item != null)
        {
            if (item.scriptableItem != null)
            {
                if (GameManager.Instance.GameInfo.PlayerInfo.Money >= item.scriptableItem.Price)
                {
                    return true;
                }
            }
            else
            {
                if (GameManager.Instance.GameInfo.PlayerInfo.Money >= item.pieceprice)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Metodo per caricare scriptable objects di un tipo specifico da una cartella
    public void LoadScriptableObjects<T>(string folderPath, Dictionary<string, T> scriptableObjectDict) where T : ScriptableObject
    {
        T[] allitems = Resources.LoadAll<T>(folderPath);

        foreach (T item in allitems)
        {
            scriptableObjectDict[$"{folderPath}/{item.name}"] = item;
            Debug.Log($"Caricato: {item.name} da percorso Resources/{folderPath}/{item.name}.asset");
        }
    }

    public void FilterLockedItems()
    {
        // Questo metodo prende tutti gli asset caricati prima dalle cartelle alle liste (item del gioco)
        // e li confronta con il file di salvataggio del player, che contiene solo gli item sbloccati
        // Controllare anche per evitare di prendere item che il giocatore ha già negli "slot" (questo vale nello specifico per i manuali)
    }
    public void SelectRandomManuals(int numberOfManuals, float padding)
    {
        var tempList = new List<KeyValuePair<string, ScriptableManual>>(scriptableManualDict);

        if (tempList.Count < numberOfManuals)
        {
            Debug.LogError("Non ci sono abbastanza manuali nella lista!");
            return;
        }

        float planeLength = plane_manuals.GetComponent<Renderer>().bounds.size.x;
        float totalRequiredSpace = numberOfManuals * padding;

        if (planeLength < totalRequiredSpace)
        {
            Debug.LogError("Non c'è abbastanza spazio per posizionare i manuali con il padding richiesto.");
            return;
        }

        float spacing = (planeLength - (padding * (numberOfManuals - 1))) / (numberOfManuals + 1);
        Vector3 planeStartPosition = plane_manuals.transform.position;
        float planeMinX = planeStartPosition.x - planeLength / 2;

        for (int i = 0; i < numberOfManuals; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempList.Count);
            var selectedPair = tempList[randomIndex];
            string assetPath = selectedPair.Key;
            ScriptableManual selectedManual = selectedPair.Value;

            Vector3 position = new Vector3(
                planeMinX + spacing * (i + 1) + padding * i,
                planeStartPosition.y,
                planeStartPosition.z
            );

            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(70, 100), 0);

            GameObject obj = Instantiate(selectedManual.Prefab, position, rotation);
            ItemData item = obj.GetComponent<ItemData>();
            if (item != null)
            {
                item.scriptableItem = selectedManual;
                item.ScriptableItemPath = assetPath; // Set the path
                item.shopScaling = true;
                item.bought = false;

            }

            manuals.Add(obj);
            tempList.RemoveAt(randomIndex);

            Debug.Log("Selezionato manuale: " + selectedManual.name + " alla posizione " + position);
        }
    }

    public void SelectRandomConsumables(int numberOfConsumables, float padding)
    {
        var tempList = new List<KeyValuePair<string, ScriptableConsumable>>(scriptableConsumableDict);

        if (tempList.Count < numberOfConsumables)
        {
            Debug.LogError("Non ci sono abbastanza consumabili nella lista!");
            return;
        }

        float planeLength = plane_consumable.GetComponent<Renderer>().bounds.size.x;
        float totalRequiredSpace = numberOfConsumables * padding;

        if (planeLength < totalRequiredSpace)
        {
            Debug.LogError("Non c'è abbastanza spazio per posizionare i consumabili con il padding richiesto.");
            return;
        }

        float spacing = (planeLength - (padding * (numberOfConsumables - 1))) / (numberOfConsumables + 1);
        Vector3 planeStartPosition = plane_consumable.transform.position;
        float planeMinX = planeStartPosition.x - planeLength / 2;

        for (int i = 0; i < numberOfConsumables; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempList.Count);
            var selectedPair = tempList[randomIndex];
            string assetPath = selectedPair.Key;
            ScriptableConsumable selectedConsumable = selectedPair.Value;

            Vector3 position = new Vector3(
                planeMinX + spacing * (i + 1) + padding * i,
                planeStartPosition.y,
                planeStartPosition.z
            );

            GameObject obj = Instantiate(selectedConsumable.Prefab, position, Quaternion.identity);
            ItemData item = obj.GetComponent<ItemData>();
            if (item != null)
            {
                item.scriptableItem = selectedConsumable;
                item.ScriptableItemPath = assetPath; // Set the path
                item.shopScaling = true;
                item.bought = false;
            }

            obj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(70, 100), 0);
            consumables.Add(obj);
            tempList.RemoveAt(randomIndex);

            Debug.Log("Selezionato consumabile: " + selectedConsumable.name + " alla posizione " + position);
        }
    }

    public void SelectRandomPieces(int numberOfPieces, float padding)
    {
        // Generate pieces for the current level
        List<PieceData> pieces = LevelGenerator.Instance.GeneratePieces(
            "Pieces", "Modifiers", PieceColor.White,
            GameManager.Instance.GameInfo.currentLevel,
            GameManager.Instance.GameInfo.currentStage,
            numberOfPieces,
            numberOfPieces
        );

        if (pieces.Count < numPieces)
        {
            Debug.LogError($"not enough pieces to populate shop section. Pieces needed:{numberOfPieces}, pieces found:{pieces.Count}");
            return;
        }

        float planeLength = plane_pieces.GetComponent<Renderer>().bounds.size.x;
        float totalRequiredSpace = numberOfPieces * padding;

        if (planeLength < totalRequiredSpace)
        {
            Debug.LogError("Non c'è abbastanza spazio per posizionare i pezzi con il padding richiesto.");
            return;
        }

        float spacing = (planeLength - (padding * (numberOfPieces - 1))) / (numberOfPieces + 1);
        Vector3 planeStartPosition = plane_pieces.transform.position;
        float planeMinX = planeStartPosition.x - planeLength / 2;

        for (int i = 0; i < pieces.Count; i++)
        {
            PieceData pieceData = pieces[i];
            GameObject prefab = FindPiecePrefabById(pieceData.PrefabID);

            if (prefab == null)
            {
                Debug.LogError("Prefab non trovato per ID: " + pieceData.PrefabID);
                continue;
            }

            Vector3 position = new Vector3(
                planeMinX + spacing * (i + 1) + padding * i,
                planeStartPosition.y,
                planeStartPosition.z
            );

            GameObject obj = Instantiate(prefab, position, Quaternion.identity);
            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            // Remove unnecessary components
            Destroy(obj.GetComponentInChildren<ParticleSystem>());
            Destroy(obj.GetComponent<PieceStatus>());
            Destroy(obj.GetComponent<DraggableBehaviour>());

            // Add and configure ItemData
            ItemData itemData = obj.AddComponent<ItemData>();
            if (itemData != null)
            {
                itemData.bought = false;
                itemData.shopScaling = true;
                itemData.el = 0.1f;
                itemData.pieceData = pieceData;
                // If you have a path for the piece, set ScriptableItemPath here
                // itemData.ScriptableItemPath = "path/to/scriptablePiece";
            }

            Renderer objRenderer = obj.GetComponent<Renderer>();
            if (objRenderer != null)
            {
                float objHeight = objRenderer.bounds.size.y / 10;
                position.y = planeStartPosition.y + objHeight / 2;
                obj.transform.position = position;
            }
            else
            {
                Debug.LogWarning("Il prefab del pezzo non ha un Renderer. Non posso calcolare la sua altezza.");
            }

            obj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(70, 100), 0);

            Debug.Log("Pezzo selezionato: " + pieceData.PrefabID + " alla posizione " + position);
        }
    }
    // Metodo per cercare il prefab giusto in Resources/Pieces
    private GameObject FindPiecePrefabById(int prefabId)
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Pieces");
        foreach (GameObject prefab in prefabs)
        {
            PieceStatus pieceStatus = prefab.GetComponent<PieceStatus>();
            if (pieceStatus != null && pieceStatus.PrefabID == prefabId)
            {
                return prefab;
            }
        }
        return null;
    }
}