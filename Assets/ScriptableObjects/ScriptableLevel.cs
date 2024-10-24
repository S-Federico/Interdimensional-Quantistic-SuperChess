using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Scriptable Level", menuName = "ScriptableObjects/ScriptableLevel")]
public class ScriptableLevel : ScriptableObject 
{
    public string Name;
    public List<GameObject> Prefabs;  
    public List<ScriptableStatusModifier> Modifiers;
    public Scene scene;
}
