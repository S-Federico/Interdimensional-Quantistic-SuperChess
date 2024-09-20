using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
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
    public GameObject priceTag;
    public GameObject sellTag;
    public GameObject useTag;

    //Ancore per i bottoni
    Vector3 bottomLeft;
    Vector3 bottomRight;
    Vector3 bottomCenter;

    public void Start()
    {
        // Calcola il bounding box del modello del GameObject padre (assumendo che il modello abbia un MeshRenderer)
        Renderer modelRenderer = this.gameObject.GetComponentInChildren<Renderer>();
        if (modelRenderer != null)
        {
            float objHeight = modelRenderer.bounds.size.y;
            float objWidth = modelRenderer.bounds.size.x;
            float objLenght = modelRenderer.bounds.size.z;

            bottomCenter = new Vector3();
            bottomCenter.z = this.gameObject.transform.position.z - (objLenght / 2) - 0.06f;
            bottomCenter.y = this.gameObject.transform.position.y;
            bottomCenter.x = this.gameObject.transform.position.x;

            bottomLeft = new Vector3();
            bottomLeft.z = this.gameObject.transform.position.z - (objLenght / 2) - 0.06f;
            bottomLeft.y = this.gameObject.transform.position.y;
            bottomLeft.x = this.gameObject.transform.position.x - 0.07f;

            bottomRight = new Vector3();
            bottomRight.z = this.gameObject.transform.position.z - (objLenght / 2) - 0.06f;
            bottomRight.y = this.gameObject.transform.position.y;
            bottomRight.x = this.gameObject.transform.position.x + 0.07f;

        }
        else
        {
            Debug.LogError("Non è stato trovato un Renderer nel GameObject padre o nei suoi figli.");
        }

        el = 0.1f;
        float sellprice = scriptableItem.Price / 2;

        buyTag = Instantiate(Resources.Load<GameObject>("Prefabs/ButtonPrefab"));
        sellTag = Instantiate(Resources.Load<GameObject>("Prefabs/ButtonPrefab"));
        useTag = Instantiate(Resources.Load<GameObject>("Prefabs/ButtonPrefab"));
        priceTag = Instantiate(Resources.Load<GameObject>("Prefabs/ButtonPrefab"));


        SetButton(buyTag, bottomLeft, ButtonType.Buy, "Buy");
        SetButton(sellTag, bottomLeft, ButtonType.Sell, $"Sell ({sellprice}$)");
        SetButton(useTag, bottomRight, ButtonType.Use, "Use");
        SetButton(priceTag, bottomRight, ButtonType.PriceTag, $"{scriptableItem.Price}$");
    }

    public void SetButton(GameObject b, Vector3 anchor, ButtonType type, string text)
    {
        b.transform.position = this.gameObject.transform.position;
        b.transform.position = anchor;

        // Imposta il padre del bottone
        b.transform.SetParent(transform);
        b.SetActive(false);

        //Imposta il testo
        b.GetComponentInChildren<TextMeshProUGUI>().text = text;
        //Imposta il buttontype
        b.GetComponent<ButtonBehaviour>().associatedItem = this;
        b.GetComponent<ButtonBehaviour>().type = type;
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
        sellTag.SetActive(false);
        useTag.SetActive(false);
        priceTag.SetActive(false);
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
        if (bought)
        {
            sellTag.SetActive(true);
            useTag.SetActive(true);
        }
        else
        {
            buyTag.SetActive(true);
            priceTag.SetActive(true);
        }
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
