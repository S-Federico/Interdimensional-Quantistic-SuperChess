using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemData : MonoBehaviour,IClickable
{   
    public ScriptableItem scriptableItem;

    public bool bought;
    public void OnClick()
    {
        if (!bought){
            ShopManager shop=GameObject.Find("ShopManager").GetComponent<ShopManager>();
            shop.BuyItem(this);
        }

        //mostra ui con azioni possibili
        //se bought sono usa e vendis
        //se non bought sono compra e cazzi
        //se clicco su compra chiama shopmanager.buy
        Debug.Log("CIAO CHI E'? IO SONO "+scriptableItem.name);
    }
}
