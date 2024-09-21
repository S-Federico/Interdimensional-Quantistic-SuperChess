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
        player= GameObject.Find("Player").GetComponent<PlayerManager>();

    }

    public void BuyItem(ItemData item)
    {
        if (item != null)
        {
            if(CanBeBought(item)){
                player.Money-=item.scriptableItem.Price;
                //istanzia nell'inventario del player
                player.inventory.Add(item);
                item.bought=true;
                //cambia la transform (prima o poi sarà responsabilità del player o dell'inventario stesso)
                item.gameObject.transform.position=GameObject.Find("PlayerInventory").transform.position;
            }
        }
    }

    public bool CanBeBought(ItemData item){
        if (item != null)
        {
            if (player.Money >= item.scriptableItem.Price)
            {
                return true;
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

    public void FillShop()
    {
        // prendere tre elementi dalla lista da piazzare
        // istanziare ognuno degli elementi con la transform corretta 
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
        List<ScriptablePiece> tempList = new List<ScriptablePiece>(scriptablePieceList);
        if (tempList.Count < numberOfPieces)
        {
            Debug.LogError("Non ci sono abbastanza manuali nella lista!");
            return;
        }
        float planeLength = plane_pieces.GetComponent<Renderer>().bounds.size.x;
        float totalRequiredSpace = numberOfPieces * padding;
        if (planeLength < totalRequiredSpace)
        {
            Debug.LogError("Non c'è abbastanza spazio per posizionare i manuali con il padding richiesto.");
            return;
        }
        float spacing = (planeLength - (padding * (numberOfPieces - 1))) / (numberOfPieces + 1);
        Vector3 planeStartPosition = plane_pieces.transform.position;
        float planeMinX = planeStartPosition.x - planeLength / 2;
        for (int i = 0; i < numberOfPieces; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempList.Count);
            ScriptablePiece selectedPiece = tempList[randomIndex];
            Vector3 position = new Vector3(
                planeMinX + spacing * (i + 1) + padding * i,
                planeStartPosition.y,
                planeStartPosition.z
            );
            GameObject obj = Instantiate(selectedPiece.Prefab, position, Quaternion.identity);
            ItemData item = obj.GetComponent<ItemData>();
            if (item != null)
            {
                item.scriptableItem = selectedPiece;
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
            pieces.Add(obj);
            tempList.RemoveAt(randomIndex);
            Debug.Log("Selezionato pezzo: " + selectedPiece.name + " alla posizione " + position);
        }
    }

}