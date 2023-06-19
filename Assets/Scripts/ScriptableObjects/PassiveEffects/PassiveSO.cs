using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "SO/Passive")]
public class PassiveSO : ScriptableObject
{
    [Header("Basic")]
    public int id;
    public string moveName;
    public string moveDescription;

    [Header("Passive")]
    public List<Passive> passiveActions = new List<Passive>();

}


[System.Serializable]
public class Passive
{
    public string name;
    public EffectSO effect;
    public PassiveTargets targets;
    public PassiveConditions conditions;
}

[System.Serializable]
public class PassiveConditions
{
    public bool always = true;
    public bool whenHit;
    public bool whenTagIn;
    public bool whenTagOut;
    public bool whenCrit;
    public bool whenSpecial;

    public bool enemyCloseToDeath;
    public bool whenEnemyHit;
    public bool whenEnemyStunned;
}

[System.Serializable]
public class PassiveTargets
{
    public bool team;
    public bool enemy;
}

