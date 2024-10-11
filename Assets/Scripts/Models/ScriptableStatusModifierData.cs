using Array2DEditor;
using UnityEngine;

public class ScriptableModifierData
{
    public string name;
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

    // Build Data from ScriptableObject
    public static ScriptableModifierData FromScriptableObject(ScriptableStatusModifier scriptableStatusModifier)
    {
        ScriptableModifierData data = new ScriptableModifierData
        {
            attributeType = scriptableStatusModifier.attributeType,
            modifierType = scriptableStatusModifier.modifierType,
            applicationType = scriptableStatusModifier.applicationType,
            statusEffectType = scriptableStatusModifier.statusEffectType,
            areaOfEffect = scriptableStatusModifier.areaOfEffect,
            durationType = scriptableStatusModifier.durationType,
            icon = scriptableStatusModifier.icon,
            value = scriptableStatusModifier.value,
            areaSize = scriptableStatusModifier.areaSize,
            effectDuration = scriptableStatusModifier.effectDuration,
            matrixValue = scriptableStatusModifier.matrixValue,
            name = scriptableStatusModifier.name
        };
        
        return data;
    }
}
