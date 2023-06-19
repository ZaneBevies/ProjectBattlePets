using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "SO/Move", order = 5)]
public class MoveSO : ScriptableObject
{
    [Header("Basic")]
    public int id;
    public string moveName;
    public string moveDescription;
    public MoveSet moveSet;
    public Sprite iconSprite;
    public float baseCooldown = 1f;

    [Header("Move")]
    public List<Move> moveActions = new List<Move>();
    

   

    public FireProjectileEffectSO GetProjectile(bool inAir, int hp)
    {
        foreach (Move move in moveActions)
        {
            if (move.effect.effectType == EffectType.FireProjectile)
            {
                if (move.conditions.whenUsed && move.conditions.whenInAir == inAir)
                {
                    if (move.conditions.whenHP.active)
                    {
                        if (move.conditions.whenHP.HPInequality == Inequality.Equal)
                        {
                            if (hp == move.conditions.whenHP.value)
                            {
                                return move.effect as FireProjectileEffectSO;
                            }
                        }
                        else if (move.conditions.whenHP.HPInequality == Inequality.GreaterThan)
                        {
                            if (hp > move.conditions.whenHP.value)
                            {
                                return move.effect as FireProjectileEffectSO;
                            }
                        }
                        else if (move.conditions.whenHP.HPInequality == Inequality.GreaterThanOrEqual)
                        {
                            if (hp >= move.conditions.whenHP.value)
                            {
                                return move.effect as FireProjectileEffectSO;
                            }
                        }
                        else if (move.conditions.whenHP.HPInequality == Inequality.LessThan)
                        {
                            if (hp < move.conditions.whenHP.value)
                            {
                                return move.effect as FireProjectileEffectSO;
                            }
                        }
                        else if (move.conditions.whenHP.HPInequality == Inequality.LessThanOrEqual)
                        {
                            if (hp <= move.conditions.whenHP.value)
                            {
                                return move.effect as FireProjectileEffectSO;
                            }
                        }
                    }
                    else
                    {
                        return move.effect as FireProjectileEffectSO;
                    }
                    
                }
                
            }
        }

        return null;

    }

}






[System.Serializable]
public class Move
{
    public string name;
    public EffectSO effect;
    public Targets targets;
    public Conditions conditions;
}

[System.Serializable]
public class Conditions
{
    public bool whenUsed;
    public bool whenNotInAir = true;
    public bool whenInAir;
    public bool whenTagIn;
    public bool whenTagOut;
    public InequalityCondition whenHP;
}

[System.Serializable]
public class Targets
{
    public bool self;
    public bool team;
    public bool enemy;
}

[System.Serializable]
public class InequalityCondition
{
    public bool active;
    public Inequality HPInequality;
    public float value;
}


public enum Inequality
{
    Equal,
    LessThan,
    GreaterThan,
    LessThanOrEqual,
    GreaterThanOrEqual
}

public enum MoveSet
{
    Basic,
    Special
}




