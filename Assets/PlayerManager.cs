using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<ItemData> inventory;
    public int Money;
    public List<GameObject> TemporaryManuals;
    public List<ItemData> PManuals;
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
    }

}