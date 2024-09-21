using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour, IClickable
{
    public ButtonType type;
    public ItemData associatedItem;

    public void Start(){
        this.gameObject.GetComponent<Canvas>().worldCamera=Camera.main;
    }
    public void OnClick()
    {
        associatedItem.OnButtonClicked(type);
    }
}
