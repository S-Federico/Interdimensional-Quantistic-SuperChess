using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableItem : ScriptableObject 
{
    public string Name;
    public string Description;
    public GameObject Prefab;  
    public int Price;
    public List<ScriptableStatusModifier> Modifiers;

}
