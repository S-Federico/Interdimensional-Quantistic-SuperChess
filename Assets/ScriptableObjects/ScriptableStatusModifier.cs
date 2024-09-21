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

    public void Apply(PieceStatus piece, BoardManager boardManager)
    {
        switch (modifierType)
        {
            case ModifierType.AttributeModifier:
                switch (attributeType)
                {
                    case AttributeType.Hp:
                        ApplyModifier(ref piece.Hp);
                        break;
                    case AttributeType.Attack:
                        ApplyModifier(ref piece.Attack);
                        break;
                    case AttributeType.Cure:
                        ApplyModifier(ref piece.cure);
                        break;
                    case AttributeType.HitChance:
                        ApplyModifier(ref piece.hitChance);
                        break;
                    case AttributeType.NumberOfMoves:
                        ApplyModifier(ref piece.NumberOfMoves);
                        break;
                    case AttributeType.AttackMatrix:
                        ApplyModifier(ref piece.AttackMatrix);
                        break;
                    case AttributeType.MovementMatrix:
                        ApplyModifier(ref piece.AddedMovements);
                        break;
                }
                break;
        }

    }


    public void Rollback(PieceStatus )
    {

    }

    public void ApplyModifier(ref int[,] value)
    {
        switch (applicationType)
        {
            case ModifierApplicationType.Additive:
                break;
            case ModifierApplicationType.Multiplicative:
                break;
            case ModifierApplicationType.Absolute:
                value = Utility.ConvertA2DintToIntMatrix(matrixValue);
                break;
        }
    }

    public void ApplyModifier(ref int value)
    {
        switch (applicationType)
        {
            case ModifierApplicationType.Additive:
                value += (int)this.value;
                break;
            case ModifierApplicationType.Multiplicative:
                value = (int)(value * this.value);
                break;
            case ModifierApplicationType.Absolute:
                value = (int)this.value;
                break;
        }
    }

    public void ApplyModifier(ref double value)
    {
        switch (applicationType)
        {
            case ModifierApplicationType.Additive:
                value += this.value;
                break;
            case ModifierApplicationType.Multiplicative:
                value = value * this.value;
                break;
            case ModifierApplicationType.Absolute:
                value = this.value;
                break;
        }
    }


}
