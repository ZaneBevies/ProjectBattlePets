using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleNode : Node
{
    [Header("Item Drops")]
    public List<ItemDrop> itemDrops = new List<ItemDrop>();

    [Header("Monster Spawn")]
    public Sprite backgroundSprite;
    public List<MonsterSpawn> monsterPool = new List<MonsterSpawn>();
    [Range(1,5)]public int minSpawn = 1;
    [Range(1,5)]public int maxSpawn = 1;

    private List<MonsterSpawn> mons = new List<MonsterSpawn>();
    //public MonsterSO monster;
    //public int level = 1;

    public bool completed = false;
    public int completedObjectiveUnlock = 0; //0 is nothing 1 and above are different objectives that get activated when completing

    public List<ObjectiveGate> unlockGates = new List<ObjectiveGate>();

    [Header("Default Settings")]
    public Sprite incompleteSprite;
    public Sprite completeSprite;
    public override void SetComplete(bool state)
    {
        completed = state;

        for (int i = 0; i < unlockGates.Count; i++)
        {
            if (completed)
            {
                unlockGates[i].Open();
            }
            else
            {
                unlockGates[i].Close();
            }
        }

        if (completed)
        {
            GetComponent<SpriteRenderer>().sprite = completeSprite;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = incompleteSprite;
        }

        if (completedObjectiveUnlock > 0 && completed)
        {
            GM.objectivesComplete[completedObjectiveUnlock - 1] = true;
        }
    }
    public override void OnEnter() // Entering node
    {
        
        if (!completed)
        {
            if (cutscene != null)
            {
                GM.cutsceneController.PlayCutscene(cutscene);
            }

            GM.playerManager.OnEnterNode(text, this);
        }
        else
        {
            GM.playerManager.OnEnterNode(doneText, this);
        }
    }

    public override void OnExit(Node previousNode, Node currentNode, Node newNode)
    {
        if (previousNode == null || previousNode != newNode) 
        {
            newNode.OnEnter();
        }
        else 
        {
            newNode.OnEnter();
        }
    }

    public override void OnInteract() 
    {
        if (GM.playerHP <= 0) { return; }

        int spawnNum = Random.Range(minSpawn, maxSpawn);

        mons = new List<MonsterSpawn>();

        for (int i = 0; i < spawnNum; i++)
        {
            int total = 0;

            for (int j = 0; j < monsterPool.Count; j++)
            {
                total += monsterPool[j].weight;
            }

            float random = Random.Range(1, total);



            int addUp = 0;
            for (int j = 0; j < monsterPool.Count; j++)
            {
                addUp = addUp + monsterPool[j].weight;

                if (random <= addUp)
                {
                    mons.Add(monsterPool[j]);
                    break;
                }
            }

        }

        List<StoredItem> rewardedItems = new List<StoredItem>();

        for (int i = 0; i < itemDrops.Count; i++)
        {
            float random = Random.Range(0f, 100f);

            if (itemDrops[i].chance >= random)
            {
                int randomAmount = Random.Range(itemDrops[i].minDrops, itemDrops[i].maxDrops);

                rewardedItems.Add(new StoredItem(itemDrops[i].item, randomAmount));
            }
        }


        if (mons != null)
        {
            GM.overworldGameobject.SetActive(false);
            GM.overworldUI.gameObject.SetActive(false);

            GM.battleManager.InitBattle(mons, nodeType, backgroundSprite, rewardedItems);
        }


        

        

    }

    public override bool IsComplete()
    {
        return completed;
    }

    public override void Refresh()
    {
        if (!completed)
        {
            GM.playerManager.OnRefreshNode(text);
        }
        else
        {
            GM.playerManager.OnRefreshNode(doneText);
        }
    }
}

[System.Serializable]
public class MonsterSpawn
{
    public MonsterSO monster;
    public int minLevel, maxLevel;
    public int weight;
}


[System.Serializable]
public class ItemDrop
{
    public MonsterItemSO item;
    [Range(0f, 100f)]public float chance;
    public int minDrops, maxDrops;
}
