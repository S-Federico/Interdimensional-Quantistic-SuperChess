using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Manual",menuName ="ScriptableObjects/ScriptableManual")]
public class ScriptableManual : ScriptableObject 
{
    public string Name;
    public GameObject Prefab;    
}
