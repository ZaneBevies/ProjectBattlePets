using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIActionData", menuName = "SO/AI/ActionData")]
public class ActionDataSO : ScriptableObject
{
    public List<ActionData> actionDatas = new List<ActionData>();

    void Awake()
    {
        if (actionDatas.Count <= 0)
        {
            actionDatas.Add(new ActionData("Pause", 50));
            actionDatas.Add(new ActionData("Attack", 15));
            actionDatas.Add(new ActionData("Special", 15));
            actionDatas.Add(new ActionData("HighJump", 5));
            actionDatas.Add(new ActionData("MediumJump", 5));
            actionDatas.Add(new ActionData("LowJump", 5));
            actionDatas.Add(new ActionData("Swap", 5));
        }
    }


    public string GetMove(bool punkBattle)
    {
        List<ActionData> datas = new List<ActionData>();
        datas.Add(actionDatas[0]);
        datas.Add(actionDatas[1]);
        datas.Add(actionDatas[2]);
        datas.Add(actionDatas[3]);
        datas.Add(actionDatas[4]);
        datas.Add(actionDatas[5]);

        if (punkBattle)
        {
            datas.Add(actionDatas[6]);
        }

        // GET TOTAL WEIGHT
        float total = 0;

        for (int i = 0; i < datas.Count; i++)
        {
            total += datas[i].weight;
        }

        // PICK RANDOM NUM BETWEEN 1 AND TOTAL
        float random = Random.Range(1, total);

        // FIND THE INDEX FOR RANDOM NUMBER
        int actionIndex = 0;
        float addUp = 0;
        for (int i = 0; i < datas.Count; i++)
        {
            addUp = addUp + datas[i].weight;

            if (random <= addUp)
            {
                actionIndex = i;
                break;
            }
        }

        // RETURN MOVE NAME FROM SELECTED INDEX
        return datas[actionIndex].name;
    }
}

[System.Serializable]
public class ActionData
{
    public string name;
    public int weight;

    public ActionData(string n, int w)
    {
        name = n;
        weight = w;
    }
}
