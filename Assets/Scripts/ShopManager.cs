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
    //      {Probabilmente saranno di 3 tipi: consumabili, non consumabili e pezzi speciali}

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

    //placeholder per il gameobject o chi per lui

    public GameObject plane_manuals;
    public GameObject plane_pieces;
    public GameObject plane_consumable;
    public List<ScriptableManual> scriptableManualList = new List<ScriptableManual>();
    public List<GameObject> manuals = new List<GameObject>();

    void Start()
    {
        Debug.Log("SONO LO SHOP MANAGER");
        LoadScriptableManuals();
        SelectRandomManuals(3,0.1f);
    }

    // Metodo per caricare tutti gli ScriptableObject presenti nella cartella
    public void LoadScriptableManuals()
    {
        // Percorso della cartella dove si trovano gli asset
        string folderPath = "Assets/ScriptableObjects/Manuals";

        // Trova tutti gli asset nella cartella
        string[] assetGUIDs = AssetDatabase.FindAssets("t:ScriptableManual", new[] { folderPath });

        // Cicla su ogni GUID trovato e carica l'asset
        foreach (string guid in assetGUIDs)
        {
            // Ottieni il percorso dell'asset
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Carica l'asset come ScriptableObject
            ScriptableManual obj = AssetDatabase.LoadAssetAtPath<ScriptableManual>(assetPath);

            // Se l'asset è stato trovato e caricato, aggiungilo alla lista
            if (obj != null)
            {
                scriptableManualList.Add(obj);
                Debug.Log("Caricato: " + obj.name);
            }
            else
            {
                Debug.LogError("Non è stato possibile caricare l'asset: " + assetPath);
            }
        }
    }

    public void FilterLockedItems()
    {
        // Questo metodo prende tutti gli asset caricati prima dalle cartelle alle liste (item del gioco)
        // e li confronta con il file di salvataggio del player, che contiene solo gli item sbloccati
    }

    public void FillShop()
    {
        // prendere tre elementi dalla lista da piazzare
        // istanziare ognuno degli elementi con la transform corretta 
    }

    // Metodo per selezionare 3 elementi casuali
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
            //Quaternion rotation = Quaternion.Euler(0, 90, 0);

            // Istanziamo il manuale selezionato
            GameObject obj = Instantiate(selectedManual.Prefab, position, rotation);

            // Aggiungiamo l'elemento alla lista dei selezionati
            manuals.Add(obj);

            // Rimuoviamo l'elemento dalla lista temporanea per evitare duplicati
            tempList.RemoveAt(randomIndex);

            // Stampa per debug
            Debug.Log("Selezionato manuale: " + selectedManual.name + " alla posizione " + position);
        }
    }


}