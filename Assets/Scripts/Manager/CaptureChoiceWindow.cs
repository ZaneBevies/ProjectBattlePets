using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaptureChoiceWindow : MonoBehaviour
{
    public GameManager GM;

    public GameObject firstObject;
    public Image dynamicImg;
    public Image staticImg;
    public Image variantImg;

    public GameObject symObject;
    public GameObject paraObject;
    public GameObject nameObject;
    public TMP_InputField inputField;

    public TextMeshProUGUI levelText;
    public GameObject amountsObject;
    public TextMeshProUGUI amountsText;

    public GameObject lvlsParent;
    public GameObject statPickParent;

    public TextMeshProUGUI typeText;

    public StatLeveler statLeveler;
    public Monster monster;
    public EnemyMonsterController enemyMonsterController;
    public void Init(Monster mon, EnemyMonsterController controller)
    {
        firstObject.SetActive(true);
        GM.battleUI.gameObject.SetActive(false);
        GM.battleManager.PauseControls();
        monster = mon;
        enemyMonsterController = controller;

        dynamicImg.sprite = monster.dynamicSprite;
        dynamicImg.color = monster.colour.colour;
        staticImg.sprite = monster.staticSprite;
        variantImg.sprite = monster.variant.variantStillSprite;

        levelText.text = "Level " + mon.level.ToString();

        symObject.SetActive(true);
        paraObject.SetActive(true);
        nameObject.SetActive(false);
        lvlsParent.SetActive(true);
        amountsObject.SetActive(false);
        statPickParent.SetActive(false);
        
    }

    public void UpdateLevelAmounts(string type)
    {
        int firstAddedAmount = 3;
        int secondAddedAmount = 2;

        if (type == "Symbiotic")
        {
            firstAddedAmount = 3;
            secondAddedAmount = 2;
        }
        else if (type == "Parasitic")
        {
            firstAddedAmount = 2;
            secondAddedAmount = 1;
        }

        if (monster.level == 1)
        {
            amountsObject.SetActive(false);
        }
        else if (monster.level <= 20 && monster.level > 1)
        {
            amountsObject.SetActive(true);
            if (monster.level - 1 == 1)
            {
                amountsText.text = (monster.level - 1).ToString() + " Level of +" + firstAddedAmount;
            }
            else
            {
                amountsText.text = (monster.level - 1).ToString() + " Levels of +" + firstAddedAmount;
            }
            
        }
        else if (monster.level > 20)
        {
            amountsObject.SetActive(true);
            if (monster.level - 20 == 1)
            {
                amountsText.text = "19 Levels of +" + firstAddedAmount + "\n" + (monster.level - 20).ToString() + " Level of +" + secondAddedAmount;
            }
            else
            {
                amountsText.text = "19 Levels of +" + firstAddedAmount + "\n" + (monster.level - 20).ToString() + " Levels of +" + secondAddedAmount;
            }

            

        }

    }

    public void StatPick(string type)
    {
        if (type == "Symbiotic")
        {
            monster.symbiotic = true;
            typeText.text = "Symbiotic";
            typeText.color = new Color(0f, 180f, 255f);
        }
        else if (type == "Parasitic")
        {
            monster.symbiotic = false;
            typeText.text = "Parasitic";
            typeText.color = new Color(255f, 0, 0);
        }



        symObject.SetActive(false);
        paraObject.SetActive(false);
        nameObject.SetActive(false);
        lvlsParent.SetActive(false);
        statPickParent.SetActive(true);
        statLeveler.Init(monster);

    }

    public void NamePick(List<int> addedStats)
    {
        for (int i = 0; i < monster.stats.Count; i++)
        {
            if (addedStats[i] > 0)
            {
                monster.AddStat(i, addedStats[i]);
                //monster.stats[i].value += addedStats[i];
            }    
            
        }

        
        symObject.SetActive(false);
        paraObject.SetActive(false);
        nameObject.SetActive(true);
        lvlsParent.SetActive(false);
        statPickParent.SetActive(false);
        inputField.text = monster.name;
    }

    public void Fin()
    {
        monster.name = inputField.text;
        enemyMonsterController.ActivateAI(true);

        GM.collectionManager.SpawnMonsterInCollection(monster);
        GM.battleUI.gameObject.SetActive(true);
        GM.battleManager.ResumeControls();
        if (enemyMonsterController.backupMonsters.Count <= 0)
        {
            GM.playerManager.currentNode.SetComplete(true);
            GM.playerManager.currentNode.Refresh();

            enemyMonsterController.ActivateAI(false);


            GM.battleGameobject.SetActive(false);
            GM.battleUI.gameObject.SetActive(false);

            GM.overworldGameobject.SetActive(true);
            GM.overworldUI.gameObject.SetActive(true);

            GM.overworldUI.healthBar.SetHealth(GM.playerHP);
        }
        else
        {
            enemyMonsterController.SetMonsterActive();
        }

        
        firstObject.SetActive(false);
    }


}
