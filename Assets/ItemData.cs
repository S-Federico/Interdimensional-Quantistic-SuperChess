using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ItemData : MonoBehaviour, IClickable
{
    public ScriptableItem scriptableItem;

    public bool bought;
    public bool selected;
    public bool alreadyElevated;
    public float el;

    public GameObject buyTag;

    public void Start()
    {
        // Instanzia il prefab
        buyTag = Instantiate(Resources.Load<GameObject>("Prefabs/ButtonPrefab"));
        buyTag.transform.position = this.gameObject.transform.position;

        // Calcola il bounding box del modello del GameObject padre (assumendo che il modello abbia un MeshRenderer)
        Renderer modelRenderer = this.gameObject.GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
        {
            float objHeight = modelRenderer.bounds.size.y;
            float objWidth = modelRenderer.bounds.size.x;
            float objLenght = modelRenderer.bounds.size.z;

            Vector3 position = new Vector3();
            position.z = this.gameObject.transform.position.z - (objLenght / 2) - 0.02f;
            position.y = this.gameObject.transform.position.y;
            position.x = this.gameObject.transform.position.x;
            buyTag.transform.position = position;

            // Imposta il padre del bottone
            buyTag.transform.SetParent(transform);
            buyTag.SetActive(false);
        }
        else
        {
            Debug.LogError("Non è stato trovato un Renderer nel GameObject padre o nei suoi figli.");
        }

        el = 0.1f;
    }

    public void Update()
    {
        if (selected)
        {
            if (!alreadyElevated)
            {
                alreadyElevated = true;
                ElevateItem();
                ShowTags();
            }
        }
        else
        {
            if (alreadyElevated)
            {
                alreadyElevated = false;
                DeElevateItem();
                HideTags();
            }

        }
    }
    public void DeElevateItem()
    {
        Vector3 p = this.gameObject.transform.position;
        Vector3 newP = new Vector3(p.x, p.y - el, p.z);
        this.gameObject.transform.position = newP;
        Debug.Log($"Deelevated to {newP.y}");
    }
    public void HideTags()
    {
        Debug.Log($"Price hidden");
        buyTag.SetActive(false);
    }
    public void ElevateItem()
    {
        Vector3 p = this.gameObject.transform.position;
        Vector3 newP = new Vector3(p.x, p.y + el, p.z);
        this.gameObject.transform.position = newP;
        Debug.Log($"Elevated to{newP.y}");
    }
    public void ShowTags()
    {
        Debug.Log($"Price: {scriptableItem.Price}");
        buyTag.SetActive(true);
    }

    public void OnClick()
    {
        ToggleSelected();
        /*
        if (!bought)
        {
            ShopManager shop = GameObject.Find("ShopManager").GetComponent<ShopManager>();
            shop.BuyItem(this);
        }
*/
        //mostra ui con azioni possibili
        //se bought sono usa e vendi
        //se non bought sono compra e cazzi
        //se clicco su compra chiama shopmanager.buy
    }

    public void ToggleSelected()
    {
        selected = !selected;
    }
}
