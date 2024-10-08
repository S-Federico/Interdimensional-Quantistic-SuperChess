using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEditor;
using UnityEditor.Search;
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
    public List<ScriptableManual> scriptableManualList = new List<ScriptableManual>();
    public List<ScriptableConsumable> scriptableConsumableList = new List<ScriptableConsumable>();
    public List<ScriptablePiece> scriptablePieceList = new List<ScriptablePiece>();
    public List<GameObject> manuals = new List<GameObject>();
    public List<GameObject> consumables = new List<GameObject>();
    public List<GameObject> pieces = new List<GameObject>();

    private PlayerManager player;


    void Start()
    {
        Debug.Log("SONO LO SHOP MANAGER");
        LoadScriptableObjects<ScriptableManual>("Assets/ScriptableObjects/Manuals", scriptableManualList);
        SelectRandomManuals(scriptableManualList.Count, 0.1f);
        LoadScriptableObjects<ScriptableConsumable>("Assets/ScriptableObjects/Consumables", scriptableConsumableList);
        SelectRandomConsumables(scriptableConsumableList.Count, 0.1f);
        LoadScriptableObjects<ScriptablePiece>("Assets/ScriptableObjects/Pieces", scriptablePieceList);
        SelectRandomPieces(scriptablePieceList.Count, 0.1f);
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
                if (item.pieceData != null){
                    GameManager.Instance.GameInfo.PlayerInfo.ExtraPieces.Add(item.pieceData);
                    Destroy(item.gameObject);
                    return;
                }
                else
                    player.inventory.Add(item);

                item.bought = true;
                //cambia la transform (prima o poi sarà responsabilità del player o dell'inventario stesso)
                item.gameObject.transform.position = GameObject.Find("PlayerInventory").transform.position;
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
    public void LoadScriptableObjects<T>(string folderPath, List<T> scriptableObjectList) where T : ScriptableObject
    {
        // Trova tutti gli asset del tipo specificato nella cartella
        string[] assetGUIDs = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folderPath });

        // Cicla su ogni GUID trovato e carica l'asset
        foreach (string guid in assetGUIDs)
        {
            // Ottieni il percorso dell'asset
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Carica l'asset come ScriptableObject del tipo specificato
            T obj = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            // Se l'asset è stato trovato e caricato, aggiungilo alla lista
            if (obj != null)
            {
                scriptableObjectList.Add(obj);
                Debug.Log($"Caricato: {obj.name}");
            }
            else
            {
                Debug.LogError($"Non è stato possibile caricare l'asset: {assetPath}");
            }
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
        // Creiamo una copia della lista originale per non modificare quella originale
        List<ScriptableManual> tempList = new List<ScriptableManual>(scriptableManualList);

        // Controlliamo che ci siano abbastanza elementi nella lista
        if (tempList.Count < numberOfManuals)
        {
            Debug.LogError("Non ci sono abbastanza manuali nella lista!");
            return;
        }

        // Recuperiamo le dimensioni del piano
        float planeLength = plane_manuals.GetComponent<Renderer>().bounds.size.x;

        // Verifichiamo che lo spazio disponibile sia sufficiente per includere il padding tra i manuali
        float totalRequiredSpace = numberOfManuals * padding;
        if (planeLength < totalRequiredSpace)
        {
            Debug.LogError("Non c'è abbastanza spazio per posizionare i manuali con il padding richiesto.");
            return;
        }

        // Calcoliamo la distanza tra ogni manuale (incluso il padding) sull'asse X
        float spacing = (planeLength - (padding * (numberOfManuals - 1))) / (numberOfManuals + 1);  // +1 per evitare di posizionare manuali fuori dal bordo

        // Recuperiamo la posizione di partenza del piano
        Vector3 planeStartPosition = plane_manuals.transform.position;
        float planeMinX = planeStartPosition.x - planeLength / 2;

        // Ciclo per selezionare "numberOfManuals" elementi casuali
        for (int i = 0; i < numberOfManuals; i++)
        {
            // Selezioniamo un indice casuale
            int randomIndex = UnityEngine.Random.Range(0, tempList.Count);

            // Prendiamo l'elemento casuale dalla lista temporanea
            ScriptableManual selectedManual = tempList[randomIndex];

            // Calcoliamo la posizione in cui piazzare l'oggetto
            Vector3 position = new Vector3(
                planeMinX + spacing * (i + 1) + padding * i,  // Posizionamento lungo l'asse X con padding
                planeStartPosition.y,                        // Stessa altezza Y del piano
                planeStartPosition.z                         // Stessa posizione Z del piano
            );

            // Creiamo una leggera rotazione casuale sull'asse Y
            Quaternion rotation = Quaternion.Euler(0, UnityEngine.Random.Range(70, 100), 0);

            // Istanziamo il manuale selezionato
            GameObject obj = Instantiate(selectedManual.Prefab, position, rotation);
            ItemData item = obj.GetComponent<ItemData>();
            item.shopScaling = true;
            if (item != null)
            {
                item.scriptableItem = selectedManual;
            }

            // Aggiungiamo l'elemento alla lista dei selezionati
            manuals.Add(obj);

            // Rimuoviamo l'elemento dalla lista temporanea per evitare duplicati
            tempList.RemoveAt(randomIndex);

            // Stampa per debug
            Debug.Log("Selezionato manuale: " + selectedManual.name + " alla posizione " + position);
        }
    }

    public void SelectRandomConsumables(int numberOfConsumables, float padding)
    {
        List<ScriptableConsumable> tempList = new List<ScriptableConsumable>(scriptableConsumableList);
        if (tempList.Count < numberOfConsumables)
        {
            Debug.LogError("Non ci sono abbastanza manuali nella lista!");
            return;
        }
        float planeLength = plane_consumable.GetComponent<Renderer>().bounds.size.x;
        float totalRequiredSpace = numberOfConsumables * padding;
        if (planeLength < totalRequiredSpace)
        {
            Debug.LogError("Non c'è abbastanza spazio per posizionare i manuali con il padding richiesto.");
            return;
        }
        float spacing = (planeLength - (padding * (numberOfConsumables - 1))) / (numberOfConsumables + 1);
        Vector3 planeStartPosition = plane_consumable.transform.position;
        float planeMinX = planeStartPosition.x - planeLength / 2;
        for (int i = 0; i < numberOfConsumables; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempList.Count);
            ScriptableConsumable selectedConsumable = tempList[randomIndex];
            Vector3 position = new Vector3(
                planeMinX + spacing * (i + 1) + padding * i,
                planeStartPosition.y,
                planeStartPosition.z
            );
            GameObject obj = Instantiate(selectedConsumable.Prefab, position, Quaternion.identity);
            ItemData item = obj.GetComponent<ItemData>();
            item.bought = false;
            item.shopScaling = true;
            if (item != null)
            {
                item.scriptableItem = selectedConsumable;
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
        // Genera i pezzi per il livello corrente
        List<PieceData> pieces = LevelGenerator.Instance.GeneratePieces("Pieces", "Modifiers", PieceColor.White, GameManager.Instance.GameInfo.currentLevel, GameManager.Instance.GameInfo.currentStage, numberOfPieces);

        // Calcola le dimensioni del piano e la spaziatura
        float planeLength = plane_pieces.GetComponent<Renderer>().bounds.size.x;
        float totalRequiredSpace = numberOfPieces * padding;

        if (planeLength < totalRequiredSpace)
        {
            Debug.LogError("Non c'è abbastanza spazio per posizionare i pezzi con il padding richiesto.");
            return;
        }

        // Calcola la spaziatura tra i pezzi
        float spacing = (planeLength - (padding * (numberOfPieces - 1))) / (numberOfPieces + 1);
        Vector3 planeStartPosition = plane_pieces.transform.position;
        float planeMinX = planeStartPosition.x - planeLength / 2;

        // Posiziona ogni pezzo in modo equidistante
        for (int i = 0; i < numberOfPieces; i++)
        {
            PieceData pieceData = pieces[i];

            // Cerca il prefab giusto all'interno della cartella Resources/Pieces
            GameObject prefab = FindPiecePrefabById(pieceData.PrefabID);
            if (prefab == null)
            {
                Debug.LogError("Prefab non trovato per ID: " + pieceData.PrefabID);
                continue;
            }

            // Calcola la posizione
            Vector3 position = new Vector3(
                planeMinX + spacing * (i + 1) + padding * i,
                planeStartPosition.y,
                planeStartPosition.z
            );

            // Istanzia il prefab del pezzo
            GameObject obj = Instantiate(prefab, position, Quaternion.identity);
            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            // Aggiunge e configura il componente PieceStatus
            PieceStatus pieceStatus = obj.GetComponent<PieceStatus>();

            if (pieceStatus != null)
            {
                Destroy(pieceStatus);
            }

            DraggableBehaviour drag = obj.GetComponent<DraggableBehaviour>();
            if (drag != null)
            {
                Destroy(drag);
            }

            // Aggiunge e configura il componente ItemData
            ItemData itemData = obj.AddComponent<ItemData>();
            if (itemData != null)
            {
                itemData.bought = false;
                itemData.shopScaling = true;
                itemData.el = 0.1f;
                itemData.pieceData = pieceData;
            }

            // Regola la posizione sull'asse Y in base all'altezza dell'oggetto
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

            // Aggiungi un po' di rotazione casuale sull'asse Y per variare l'orientamento
            obj.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(70, 100), 0);

            // Debug per confermare la selezione del pezzo e la posizione
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