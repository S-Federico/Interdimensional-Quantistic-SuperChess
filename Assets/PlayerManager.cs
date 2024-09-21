using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public List<ItemData> inventory;
    public int Money;

    public void Start(){
        inventory = new List<ItemData>();
        Money=100;
    }

}
