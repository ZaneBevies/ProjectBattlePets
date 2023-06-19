using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    // TAKES IN PASSIVE MOVES AND MAKES THEM STAY ACTIVE

    // TAKES IN BASIC AND SPECIAL MOVES EFFECTS AND ACTIVATES THEM / APPLIES THE BUFFS, DEBUFFS

    public GameManager GM;

    public bool friendlyController = false;

    public void ProjectileTarget()
    {

    }
    public void UseMove(MoveSO move)
    {
        foreach (Move m in move.moveActions)
        {
            if (PassConditionTest(m.conditions))
            {
                if (m.effect.effectType == EffectType.CritAttacks)
                {
                    CritAttacksEffectSO newEffect = m.effect as CritAttacksEffectSO;
                    DoCritAttacks(newEffect.time, 1);
                }
                else if (m.effect.effectType == EffectType.DoT)
                {
                    DoTEffectSO newEffect = m.effect as DoTEffectSO;
                    DoDoT(newEffect.amount, newEffect.time);
                }
                else if (m.effect.effectType == EffectType.FireProjectile)
                {
                    //Debug.Log("Fired Projectile");
                    FireProjectileEffectSO newEffect = m.effect as FireProjectileEffectSO;
                    DoProjectile(newEffect.projectilePrefab, newEffect.projectileDamage, newEffect.projectileSpeed, newEffect.lifetime, newEffect.collideWithAmountOfObjects, newEffect.criticalProjectile);
                }
                else if (m.effect.effectType == EffectType.Heal)
                {
                    HealEffectSO newEffect = m.effect as HealEffectSO;
                    DoHeal(newEffect.healAmount);
                }
                else if (m.effect.effectType == EffectType.Invulnerability)
                {
                    InvulnerabilityEffectSO newEffect = m.effect as InvulnerabilityEffectSO;
                    DoInvulnerability(newEffect.invulnerableTime, newEffect.perfectGuardEffect, newEffect.perfectGuardEffectValue, m);
                }
                else if (m.effect.effectType == EffectType.RefreshCooldown)
                {
                    RefreshCooldownEffectSO newEffect = m.effect as RefreshCooldownEffectSO;
                    DoRefreshCooldown(newEffect.chance, newEffect.whatToRefresh, newEffect.amount);
                }
                else if (m.effect.effectType == EffectType.StatMod)
                {
                    StatModEffectSO newEffect = m.effect as StatModEffectSO;
                    DoStatMod(newEffect.modifierType, newEffect.stat, newEffect.amount, m.targets.team);
                }
                else if (m.effect.effectType == EffectType.Stun)
                {
                    StunEffectSO newEffect = m.effect as StunEffectSO;
                    DoStun(newEffect.stunTime, 3);
                }
                else if (m.effect.effectType == EffectType.TakingCrits)
                {
                    TakingCritsEffectSO newEffect = m.effect as TakingCritsEffectSO;
                    DoTakingCrits(newEffect.time, 2);
                }
                else if (m.effect.effectType == EffectType.TimeBomb)
                {
                    TimeBombEffectSO newEffect = m.effect as TimeBombEffectSO;
                    DoTimeBomb(newEffect.damage, newEffect.breaksOnDamage, newEffect.decayTime, newEffect.decayAmount);
                }
            }
        }
    }

    private void DoCritAttacks(float time, int type)
    {
        // Give crit attack buff to friend/enemy for amount, time, decay
        if (friendlyController)
        {
            GM.battleManager.friendlyMonsterController.friendlyBattleBuffManager.AddBuff(time, type);
        }
        else
        {
            GM.battleManager.enemyMonsterController.enemyBattleBuffManager.AddBuff(time, type);
        }
    }

    private void DoDoT(int amount, float time)
    {
        if (friendlyController)
        {
            GM.battleManager.friendlyMonsterController.friendlyBattleBuffManager.AddBuff(amount, time);
        }
        else
        {
            GM.battleManager.enemyMonsterController.enemyBattleBuffManager.AddBuff(amount, time);
        }
    }

    private void DoProjectile(GameObject projectile, int damage, float speed, float lifeTime, int collideWithAmountOfObjects, bool criticalProjectile)
    {
        float spd = 0;
        if (friendlyController)
        { 
            float amt = GM.battleManager.friendlyMonsterController.oomph + (GM.battleManager.friendlyMonsterController.oomph * ((GM.battleManager.friendlyMonsterController.friendlyBattleBuffManager.slotValues[0]) / 100));

            spd = speed + (speed * (0.02f * amt));

            int realDmg = damage;
            float fltDmg = damage;

            if (GM.battleManager.friendlyMonsterController.airCrit)
            {
                realDmg = (int)(fltDmg + (fltDmg * 0.25f));
            }

            GM.battleManager.friendlyMonsterController.FireProjectile(projectile, spd * 10, realDmg, lifeTime, collideWithAmountOfObjects, criticalProjectile);
        }
        else
        {
            float amt = GM.battleManager.enemyMonsterController.oomph + (GM.battleManager.enemyMonsterController.oomph * ((GM.battleManager.enemyMonsterController.enemyBattleBuffManager.slotValues[0]) / 100));

            spd = speed + (speed * (0.02f * amt));

            int realDmg = damage;
            float fltDmg = damage;

            if (GM.battleManager.enemyMonsterController.airCrit)
            {
                realDmg = (int)(fltDmg + (fltDmg * 0.25f));
            }


            GM.battleManager.enemyMonsterController.FireProjectile(projectile, spd * 10, realDmg, lifeTime, collideWithAmountOfObjects, criticalProjectile);
        }
    }

    private void DoHeal(int amount)
    {
        if (friendlyController)
        {
            GM.battleManager.friendlyMonsterController.Heal(amount);
        }
        else
        {
            GM.battleManager.enemyMonsterController.Heal(amount);
        }
    }

    private void DoInvulnerability(float time, PerfectGuardEffects effect, float effectValue, Move move)
    {
        if (friendlyController)
        {
            if (move.targets.self)
            {
                GM.battleManager.friendlyMonsterController.friendlyBattleBuffManager.AddBuff(time, effect, effectValue, false);
            }

            if (move.targets.team)
            {
                GM.battleManager.friendlyMonsterController.friendlyBattleBuffManager.AddBuff(time, effect, effectValue, true);
            }

            if (move.targets.enemy)
            {
                GM.battleManager.enemyMonsterController.enemyBattleBuffManager.AddBuff(time, effect, effectValue, false);
            }
            
        }
        else
        {
            GM.battleManager.enemyMonsterController.enemyBattleBuffManager.AddBuff(time, effect, effectValue, false);
        }
    }

    private void DoRefreshCooldown(float chance, AbilityType whatToRefresh, int amount)
    {
        if (friendlyController)
        {
            //DO REFRESH COOLDOWN HERE
        }
        else
        {
            //DO REFRESH COOLDOWN HERE
        }
    }

    private void DoStatMod(StatModType type, EffectedStat stat, int amount, bool team)
    {
        if (friendlyController)
        {
            GM.battleManager.friendlyMonsterController.friendlyBattleBuffManager.AddBuff(type, stat, amount, team);
        }
        else
        {
            GM.battleManager.enemyMonsterController.enemyBattleBuffManager.AddBuff(type, stat, amount, team);
        }
    }

    private void DoStun(float time, int type)
    {
        if (friendlyController)
        {
            if (GM.battleManager.friendlyMonsterController.guardOn)
            {
                GM.battleManager.friendlyMonsterController.Guard(false, PerfectGuardEffects.None, 0f, false);
            }
            else
            {
                GM.battleManager.friendlyMonsterController.friendlyBattleBuffManager.AddBuff(time, type);
            }
            
        }
        else
        {
            if (GM.battleManager.enemyMonsterController.guardOn)
            {
                GM.battleManager.enemyMonsterController.Guard(false, PerfectGuardEffects.None, 0f, false);
            }
            else
            {
                GM.battleManager.enemyMonsterController.enemyBattleBuffManager.AddBuff(time, type);
            }

        }
    }

    private void DoTakingCrits(float time, int type)
    {
        if (friendlyController)
        {
            GM.battleManager.friendlyMonsterController.friendlyBattleBuffManager.AddBuff(time, type);
        }
        else
        {
            GM.battleManager.enemyMonsterController.enemyBattleBuffManager.AddBuff(time, type);
        }
    }

    private void DoTimeBomb(int amount, bool breaksOnDamage, float time, int decay)
    {
        //DISABLED ATM ONLY ACTIVE ON PROJECTILES THEMSELVES
        Debug.Log("A move should not contain this type of move action");
    }
    private bool PassConditionTest(Conditions conditions)
    {
        bool state = false;

        if (friendlyController)
        {
            if (conditions.whenUsed == true)
            {
                state = true;
            }
            else
            {
                state = false;
            }

  

            if (conditions.whenNotInAir && GM.battleManager.friendlyMonsterController.isGrounded) // Not in air and grounded
            {
                state = true;
            }
            else if (conditions.whenInAir && !GM.battleManager.friendlyMonsterController.isGrounded) // In air and not grounded
            {
                state = true;
            }
            else
            {
                state = false;
            }

            if (conditions.whenHP.active)
            {
                if (conditions.whenHP.HPInequality == Inequality.Equal)
                {
                    if (GM.playerHP == conditions.whenHP.value)
                    {
                        state = true;
                    }
                    else
                    {
                        state = false;
                    }
                }
                else if (conditions.whenHP.HPInequality == Inequality.GreaterThan)
                {
                    if (GM.playerHP > conditions.whenHP.value)
                    {
                        state = true;
                    }
                    else
                    {
                        state = false;
                    }
                }
                else if (conditions.whenHP.HPInequality == Inequality.GreaterThanOrEqual)
                {
                    if (GM.playerHP >= conditions.whenHP.value)
                    {
                        state = true;
                    }
                    else
                    {
                        state = false;
                    }
                }
                else if (conditions.whenHP.HPInequality == Inequality.LessThan)
                {
                    if (GM.playerHP < conditions.whenHP.value)
                    {
                        state = true;
                    }
                    else
                    {
                        state = false;
                    }
                }
                else if (conditions.whenHP.HPInequality == Inequality.LessThanOrEqual)
                {
                    if (GM.playerHP <= conditions.whenHP.value)
                    {
                        state = true;
                    }
                    else
                    {
                        state = false;
                    }
                }
            }
        }
        else if (!friendlyController)
        {
            if (conditions.whenUsed == true)
            {
                state = true;
            }
            else
            {
                state = false;
            }

            if (conditions.whenNotInAir && GM.battleManager.enemyMonsterController.isGrounded) // Not in air and grounded
            {
                state = true;
            }
            else if (conditions.whenInAir && !GM.battleManager.enemyMonsterController.isGrounded) // In air and not grounded
            {
                state = true;
            }
            else
            {
                state = false;
            }

            if (conditions.whenHP.active)
            {
                if (conditions.whenHP.HPInequality == Inequality.Equal)
                {
                    if (GM.battleManager.enemyMonsterController.enemyHealth == conditions.whenHP.value)
                    {
                        state = true;
                    }
                    else
                    {
                        state = false;
                    }
                }
                else if (conditions.whenHP.HPInequality == Inequality.GreaterThan)
                {
                    if (GM.battleManager.enemyMonsterController.enemyHealth > conditions.whenHP.value)
                    {
                        state = true;
                    }
                    else
                    {
                        state = false;
                    }
                }
                else if (conditions.whenHP.HPInequality == Inequality.GreaterThanOrEqual)
                {
                    if (GM.battleManager.enemyMonsterController.enemyHealth >= conditions.whenHP.value)
                    {
                        state = true;
                    }
                    else
                    {
                        state = false;
                    }
                }
                else if (conditions.whenHP.HPInequality == Inequality.LessThan)
                {
                    if (GM.battleManager.enemyMonsterController.enemyHealth < conditions.whenHP.value)
                    {
                        state = true;
                    }
                    else
                    {
                        state = false;
                    }
                }
                else if (conditions.whenHP.HPInequality == Inequality.LessThanOrEqual)
                {
                    if (GM.battleManager.enemyMonsterController.enemyHealth <= conditions.whenHP.value)
                    {
                        state = true;
                    }
                    else
                    {
                        state = false;
                    }
                }
            }
        }

        


        return state;
    }
}
