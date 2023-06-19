using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StatModEffect", menuName = "SO/Effects/StatMod")]
public class StatModEffectSO : EffectSO
{
    [Header("Stat Modifier")]
    public StatModType modifierType;
    public EffectedStat stat;
    public int amount;

    public void Awake()
    {
        effectType = EffectType.StatMod;
    }
}

public enum StatModType
{
    Buff,
    Debuff
}

public enum EffectedStat
{
    Oomph,
    Guts,
    Juice,
    Edge,
    Wits,
    Spark
}
