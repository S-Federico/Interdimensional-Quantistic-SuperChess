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
            PConsumables.Add(go.GetComponent<ItemData>());
        }
        Debug.Log("PConsumables popolato.");
    }

}