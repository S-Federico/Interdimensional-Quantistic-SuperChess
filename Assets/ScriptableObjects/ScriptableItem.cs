using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableItem : ScriptableObject 
{
    public string Name;
    public GameObject Prefab;  
    public int Price;
    public ScriptableStatusModifier Modifier;

}
