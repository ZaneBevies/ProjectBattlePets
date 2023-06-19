using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterItem", menuName = "SO/MonsterItem")]
public class MonsterItemSO : ScriptableObject
{
    public int id;
    public string itemName;
    public string desc;
    public Sprite icon;
    public ItemType type;
    public int maxStack = 1000;

   

    public List<ItemEffect> itemEffects = new List<ItemEffect>();

}


public enum ItemType
{
    Currency, // Glitter, used to upgrade and as curreny
    Material, // Materials used to craft items and consumables
    Catalyst // can be Equipped as well as crafted into higher items
}
[System.Serializable]
public class ConditionsItem
{
    public bool onTagIn;
    public bool onTagOut;
    public bool startOfBattle;
    public bool onBeingHit;
    public bool onHit;
    public bool onCrit;
    public bool inAir;
    public InequalityCondition whenHP;
}

[System.Serializable]
public class ItemEffect
{
    public string name;
    public EffectSO effect;
    public Targets targets;
    public ConditionsItem conditions;
}