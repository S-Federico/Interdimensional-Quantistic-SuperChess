using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<ItemData> inventory;
    public int Money;
    public List<GameObject> TemporaryManuals;
    public List<GameObject> TemporaryConsumables;
    public List<ItemData> PManuals;
    public List<ItemData> PConsumables;
    public List<PieceStatus> pieces;

    public void Start()
    {
        inventory = new List<ItemData>();
        Money = 100;
        TemporaryInventoryFill();
    }

    private void TemporaryInventoryFill()
    {
        foreach (GameObject go in TemporaryManuals)
        {
            PManuals.Add(go.GetComponent<ItemData>());
        }
        Debug.Log("PManuals popolato.");
        foreach (GameObject go in TemporaryConsumables)
        {
            go.GetComponent<ItemData>().bought = true;
            PConsumables.Add(go.GetComponent<ItemData>());
            Debug.Log("PConsumables " + go.GetComponent<ItemData>().scriptableItem.Name);
        }
        Debug.Log("PConsumables popolato.");
    }

    public void RemoveItem(ItemData item)   // Da aggiornare per gestire meglio le liste (inventory etc)
    {
        bool removed = false;

        // Controllo il tipo di scriptableItem e rimuovo dalla lista appropriata
        if (item.scriptableItem is ScriptableConsumable)
        {
            ItemData consumable = PConsumables.FirstOrDefault(c => c.scriptableItem.Name == item.scriptableItem.Name);
            if (consumable != null)
            {
                PConsumables.Remove(consumable);
                removed = true;
                Debug.Log("Consumabile rimosso dalla lista.");
            }
        }
        else if (item.scriptableItem is ScriptableManual)
        {
            ItemData manual = PManuals.FirstOrDefault(m => m.scriptableItem.Name == item.scriptableItem.Name);
            if (manual != null)
            {
                PManuals.Remove(manual);
                removed = true;
                Debug.Log("Manuale rimosso dalla lista.");
            }
        }

        // Se non è stato trovato l'item in nessuna lista
        if (!removed)
        {
            Debug.Log("Item non trovato in nessuna lista.");
        }
    }
}