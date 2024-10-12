using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using Palmmedia.ReportGenerator.Core.Reporting.Builders.Rendering;

public class ShopManager : MonoBehaviour
{
    // Il compito di questo manager è quello di popolare lo shop.
    // Nello specifico va a prendere tra gli oggetti "sbloccati" dal player le cose da usare. (1)
    // Ovviamente deve far avvenire le transazioni (banalmente scalare i soldi al player ed effettivamente popolare le collezioni) (2)

    /*
        (1)
        Ci saranno due elenchi: uno di tutti gli oggetti esistenti nel gioco, l'altro per le cose che il giocatore ha sbloccato (ossia che sono già state acquistate almeno una volta o altri modi che comunque non importano adesso)
        Gli oggetti sono di tipi diversi. Al momento non sappiamo effettivamente che forma avranno, ma sicuramente avranno un TIPO. Nello shop ne servono N in totale.
        Il prezzo delle cose è ancora da definire come verrà scelto.
    */

    /*
        (2)
        Serve un controllo per garantire che il player abbia abbastanza soldi per effettuare un acquisto.
        Serve anche un metodo(?) per effettuare questo "scambio"
    */

    public GameObject plane_manuals;
    public GameObject plane_pieces;
    public GameObject plane_consumable;
    public Dictionary<string, ScriptableManual> scriptableManualDict = new Dictionary<string, ScriptableManual>();
    public Dictionary<string, ScriptableConsumable> scriptableConsumableDict = new Dictionary<string, ScriptableConsumable>();
    public List<GameObject> manuals = new List<GameObject>();
    public List<GameObject> consumables = new List<GameObject>();
    public List<GameObject> pieces = new List<GameObject>();

    private PlayerManager player;

    int numPieces=2;

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
                    Destroy(item.gameObject);
                    return;
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
                //cambia la transform (prima o poi sarà responsabilità del player o dell'inventario stesso)
                item.gameObject.transform.position = GameObject.Find("PlayerInventory").transform.position;
            }
            else
            {
                Debug.Log("Item non comprato.");
            }
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

        foreach(T item in allitems){
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

            Renderer objRenderer = obj.GetComponent<Renderer>();
            if (objRenderer != null)
            {
                float objHeight = objRenderer.bounds.size.y;
                position.y = planeStartPosition.y + objHeight / 2;
                obj.transform.position = position;
            }
            else
            {
                Debug.LogWarning("Il prefab non ha un Renderer. Non posso calcolare la sua altezza.");
            }

            obj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(70, 100), 0);
            consumables.Add(obj);
            tempList.RemoveAt(randomIndex);

            Debug.Log("Selezionato consumabile: " + selectedConsumable.name + " alla posizione " + position);
        }
    }

    public void SelectRandomPieces(int numberOfPieces, float padding)
    {
        // The logic for pieces remains the same unless you have specific paths for them.
        // If so, you can modify this method similarly to the ones above.

        // Generate pieces for the current level
        List<PieceData> pieces = LevelGenerator.Instance.GeneratePieces(
            "Pieces", "Modifiers", PieceColor.White,
            GameManager.Instance.GameInfo.currentLevel,
            GameManager.Instance.GameInfo.currentStage,
            numberOfPieces,
            numberOfPieces
        );

        if (pieces.Count <numPieces){
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