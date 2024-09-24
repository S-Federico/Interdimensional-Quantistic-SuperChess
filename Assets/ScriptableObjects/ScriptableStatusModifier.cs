using System;
using System.Collections;
using System.Collections.Generic;
using Array2DEditor;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "Modifier", menuName = "ScriptableObjects/StatusModifier")]
public class ScriptableStatusModifier : ScriptableObject
{
    public AttributeType attributeType;
    public ModifierType modifierType;
    public ModifierApplicationType applicationType;
    public StatusEffectType statusEffectType;
    public AreaOfEffect areaOfEffect;
    public DurationType durationType;
    public Sprite icon;
    public double value;
    public int areaSize;
    public int effectDuration;
    public Array2DInt matrixValue;

}
