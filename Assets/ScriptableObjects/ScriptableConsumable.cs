using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "ScriptableObjects/ScriptableConsumable")]
public class ScriptableConsumable : ScriptableItem
{
    public ConsumablesType ConsumableType;
    public Array2DInt ApplicationMatrix;
}
