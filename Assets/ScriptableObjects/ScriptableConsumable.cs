using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Consumable",menuName ="ScriptableObjects/ScriptableConsumable")]
public class ScriptableConsumable : ScriptableObject 
{
    public string Name;
    public GameObject Prefab;    
}
