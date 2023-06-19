using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBuffManager : MonoBehaviour
{
    [Header("Buff Management")]
    public GameObject buffSlotPrefab;
    public Transform locationParent;

    public GameManager GM;



    public List<BuffSlot> slots = new List<BuffSlot>();

    public List<int> slotValues = new List<int>();

    public PerfectGuardEffects perfectGuardEffect = PerfectGuardEffects.None;
    public float perfectGuardValue = 0;
    public bool perfectGuardTeam = false;

    public bool isFriendly = false;

    private float tickTimer = 0f;
    private bool tickActive = true;


    private List<PassiveSO> passives = new List<PassiveSO>();

    private bool passivesOn = false;
    void Update()
    {
        if (passivesOn)
        {
            for (int i = 0; i < passives.Count; i++)
            {

            }
        }

        if (tickActive)
        {
            if (tickTimer > 0)
            {
                tickTimer -= Time.deltaTime;
            }
            else if (tickTimer <= 0)
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i].active)
                    {
                        if (slots[i].slotType == 6) // dot
                        {
                            if (isFriendly)
                            {
                                GM.battleManager.friendlyMonsterController.TakeDamage(slotValues[6], true);
                            }
                            else
                            {
                                GM.battleManager.enemyMonsterController.TakeDamage(slotValues[6], true);
                            }
                        }

                        

                    }
                }


                tickTimer = 1f;
            }
        }
        


        
    }

    public void StartPassives(List<PassiveSO> ps)
    {
        passives = ps;
        passivesOn = true;
    }


    public void AddBuff(float time, int type) // doing crits buff / for taking crits
    {
        if (type == 1) // CritAttacks
        {
            SpawnBuffObject(9, time);
        }
        else if (type == 2) // TakingCrits
        {
            SpawnBuffObject(10, time);
        }
        else if (type == 3) // stun
        {
            SpawnBuffObject(7, time);
        }
    }

    public void AddBuff(int amount, float time) // dot
    {
        SpawnBuffObject(6, amount, time);
    }

    public void AddBuff(float time, PerfectGuardEffects effect, float effectValue, bool team) // for invulnurability and perfect guard effects
    {
        perfectGuardEffect = effect;
        perfectGuardValue = effectValue;
        perfectGuardTeam = team;
        SpawnBuffObject(8, time);
    }

    public void AddBuff(StatModType type, EffectedStat stat, int amount, bool team) // for buffing, debuffing, inversing etc any stat type
    {
        if (type == StatModType.Buff)
        {
            if (stat == EffectedStat.Oomph)
            {
                SpawnBuffObject(0, amount);
            }
            else if (stat == EffectedStat.Guts)
            {
                SpawnBuffObject(1, amount);
            }
            else if (stat == EffectedStat.Juice)
            {
                SpawnBuffObject(2, amount);
            }
            else if (stat == EffectedStat.Edge)
            {
                SpawnBuffObject(3, amount);
            }
            else if (stat == EffectedStat.Wits)
            {
                SpawnBuffObject(4, amount);
            }
            else if (stat == EffectedStat.Spark)
            {
                SpawnBuffObject(5, amount);
            }
        }
        else if (type == StatModType.Debuff)
        {
            if (stat == EffectedStat.Oomph)
            {
                SpawnBuffObject(0, -amount);
            }
            else if (stat == EffectedStat.Guts)
            {
                SpawnBuffObject(1, -amount);
            }
            else if (stat == EffectedStat.Juice)
            {
                SpawnBuffObject(2, -amount);
            }
            else if (stat == EffectedStat.Edge)
            {
                SpawnBuffObject(3, -amount);
            }
            else if (stat == EffectedStat.Wits)
            {
                SpawnBuffObject(4, -amount);
            }
            else if (stat == EffectedStat.Spark)
            {
                SpawnBuffObject(5, -amount);
            }
        }

    }

   
    private void SpawnBuffObject(int type, int amount, float time) // dot
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotType == type)
            {
                slotValues[type] += amount;
                slots[i].Merge(amount, time);
                return;
            }
        }

        slotValues[type] += amount;
        GameObject obj = Instantiate(buffSlotPrefab, locationParent);
        obj.GetComponent<BuffSlot>().Init(type, amount, time, this);
        slots.Add(obj.GetComponent<BuffSlot>());
    }

    private void SpawnBuffObject(int type, float time) // crit attacks, taking crits, stun, guard
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotType == type)
            {
                slots[i].Merge(time);
                return;
            }
        }

        GameObject obj = Instantiate(buffSlotPrefab, locationParent);
        obj.GetComponent<BuffSlot>().Init(type, time, this);
        slots.Add(obj.GetComponent<BuffSlot>());
    }

    private void SpawnBuffObject(int type, int amount) // stats
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].slotType == type)
            {
                slotValues[type] += amount;
                slots[i].MergeAmount(amount);
                return;
            }
        }

        slotValues[type] += amount;
        GameObject obj = Instantiate(buffSlotPrefab, locationParent);
        obj.GetComponent<BuffSlot>().Init(type, amount, this);
        slots.Add(obj.GetComponent<BuffSlot>());
    }

    public void ClearSlotsAfterBattle()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].TimerEnd();
        }
    }
}

public enum BuffSlotType
{
    Oomph,
    Guts,
    Juice,
    Edge,
    Wits,
    Spark,
    DoT,
    Stun,
    Guard,
    CritAttacks,
    TakingCrits,
}

