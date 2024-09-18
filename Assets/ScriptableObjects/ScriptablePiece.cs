using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Piece",menuName ="ScriptableObjects/ScriptablePiece")]
public class ScriptablePiece : ScriptableObject 
{
    public string Name;
    public GameObject Prefab;    
}
