using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<ItemData> PManuals;
    public List<ItemData> PConsumables;
    public List<PieceStatus> pieces;

    public void Start()
    {
        BuildFromData(GameManager.Instance.GameInfo.PlayerInfo);
    }

    public void BuildFromData(PlayerInfo playerInfo)
    {
        if (playerInfo == null)
        {
            Debug.LogError("playerInfo è null");
            return;
        }

        foreach (string consumPath in playerInfo.Consumables)
        {
            ScriptableConsumable obj = AssetDatabase.LoadAssetAtPath<ScriptableConsumable>(consumPath);
            if (obj == null)
            {
                Debug.LogError($"scriptableConsumable al path {consumPath} è nullo");
                return;
            }
            ItemData item = obj.Prefab.GetComponent<ItemData>();
            if (item == null)
            {
                Debug.LogError($"itemdata al path {consumPath} è nullo");
                return;
            }
            PConsumables.Add(item);
        }
        Debug.Log($"PConsumables popolato.{PConsumables.Count}");
        foreach (string manualPath in playerInfo.Manuals)
        {
            ScriptableManual obj = AssetDatabase.LoadAssetAtPath<ScriptableManual>(manualPath);
            if (obj == null)
            {
                Debug.LogError($"scriptableManual al path {manualPath} è nullo");
                return;
            }
            ItemData item = obj.Prefab.GetComponent<ItemData>();
            if (item == null)
            {
                Debug.LogError($"itemdata al path {manualPath} è nullo");
                return;
            }
            PManuals.Add(item);
        }
        Debug.Log($"PManuals popolato. {PManuals.Count}");
    }

    public void RemoveItem(ItemData item)   // Da aggiornare per gestire meglio le liste (inventory etc)
    {
        bool removed = false;
        item.bought = false;

        // Controllo il tipo di scriptableItem e rimuovo dalla lista appropriata
        if (item.scriptableItem is ScriptableConsumable)
        {
            ItemData consumable = PConsumables.FirstOrDefault(c => c.scriptableItem.Name == item.scriptableItem.Name);
            if (consumable != null)
            {
                PConsumables.Remove(consumable);
                GameManager.Instance.GameInfo.PlayerInfo.Consumables.Remove(item.ScriptableItemPath);
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
                GameManager.Instance.GameInfo.PlayerInfo.Manuals.Remove(item.ScriptableItemPath);
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