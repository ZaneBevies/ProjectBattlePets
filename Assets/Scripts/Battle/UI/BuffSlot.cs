using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffSlot : MonoBehaviour
{
    public Image typeImage;
    public TextMeshProUGUI numText;
    public TextMeshProUGUI timerText;
    public Image upDownImage;

    public int slotType;
    [HideInInspector] public int timerAmount = 0 ;

    public List<Sprite> sprites = new List<Sprite>();
    public Sprite upSprite;
    public Sprite downSprite;

    private float timer = 0;
    private bool timerOn = false;

    private BattleBuffManager manager;

    [HideInInspector] public bool active = false;
    void Update()
    {
        if (slotType == 6)
        {
            if (timerAmount > 0)
            {
                numText.text = "+" + timerAmount.ToString();
            }
            else if (timerAmount < 0)
            {
                numText.text = "-" + timerAmount.ToString();
            }
        }
       

        if (timerOn)
        {
            if (timer > 0)
            {
                string txt = Mathf.RoundToInt(timer).ToString() + "s";
                timerText.text = txt;
                timer -= Time.deltaTime;
            }
            else if (timer <= 0)
            {
                timerText.text = "";
                TimerEnd();
            }
        }
    }

    private void DoBuffs(int slotNum, bool isFriend)
    {
        if (slotNum == 7)// stun
        {
            if (isFriend)
            {
                manager.GM.battleManager.friendlyMonsterController.Stun(true, timer);
            }
            else
            {
                manager.GM.battleManager.enemyMonsterController.Stun(true, timer);
            }

        }
        else if(slotNum == 8) // guard
        {
            if (isFriend)
            {
                manager.GM.battleManager.friendlyMonsterController.Guard(true, manager.perfectGuardEffect, manager.perfectGuardValue, manager.perfectGuardTeam);
            }
            else
            {
                manager.GM.battleManager.enemyMonsterController.Guard(true, manager.perfectGuardEffect, manager.perfectGuardValue, manager.perfectGuardTeam);
            }
        }
        else if (slotNum == 9) // crit attacks
        {
            if (isFriend)
            {
                manager.GM.battleManager.friendlyMonsterController.CritAttacks(true);
            }
            else
            {
                manager.GM.battleManager.enemyMonsterController.CritAttacks(true);
            }
        }
        else if (slotNum == 10) // taking crits
        {
            if (isFriend)
            {
                manager.GM.battleManager.friendlyMonsterController.TakingCrits(true);
            }
            else
            {
                manager.GM.battleManager.enemyMonsterController.TakingCrits(true);
            }
        }





    }

    public void Init(int type, int amount, float time, BattleBuffManager man) // dot
    {
        
        typeImage.sprite = sprites[type];
        string txt = Mathf.RoundToInt(timer).ToString() + "s";
        timerText.text = txt;
        upDownImage.gameObject.SetActive(false);

        numText.text = "-" + amount.ToString();

        manager = man;
        slotType = type;
        timer = time;
        timerAmount = amount;

        DoBuffs(type, man.isFriendly);

        active = true;
        timerOn = true;
    }

    public void Init(int type, float time, BattleBuffManager man)// crit attacks, taking crits, stun, guard
    {
        if (type == 7 || type == 8)
        {
            typeImage.gameObject.SetActive(false);
            upDownImage.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
            numText.gameObject.SetActive(false);
        }
        else
        {
            typeImage.sprite = sprites[type];
            string txt = Mathf.RoundToInt(timer).ToString() + "s";
            timerText.text = txt;
            numText.text = "";


            upDownImage.color = new Color(255f, 255f, 255f, 0f);
            upDownImage.gameObject.SetActive(false);
        }


        manager = man;
        slotType = type;
        timer = time;

        DoBuffs(type, man.isFriendly);

        timerOn = true;
    }

    public void Init(int type, int amount, BattleBuffManager man) // stats
    {
        typeImage.sprite = sprites[type];
        timerText.text = "";
        upDownImage.gameObject.SetActive(false);
        if (amount >= 0)
        {
            numText.text = "+" + amount.ToString() + "%";
            numText.color = Color.green;
            //upDownImage.color = new Color(255f, 255f, 255f, 255f);
            //upDownImage.sprite = upSprite;
        }
        else
        {
            numText.text = "-" + amount.ToString() + "%";
            numText.color = Color.red;
            //upDownImage.color = new Color(255f, 255f, 255f, 255f);
            //upDownImage.sprite = downSprite;
        }

        manager = man;

        DoBuffs(type, man.isFriendly);
        slotType = type;
        timerAmount = amount;
    }

    public void Merge(int amount, float time)
    {
        timer += time;
        timerAmount += amount;

        string txt = Mathf.RoundToInt(timer).ToString() + "s";
        timerText.text = txt;

        if (timerAmount >= 0)
        {
            numText.text = "+" + timerAmount.ToString() + "%";
            numText.color = Color.green;
        }
        else
        {
            numText.text = "-" + timerAmount.ToString() + "%";
            numText.color = Color.red;
        }
    }

    public void Merge(float time)
    {
        timer += time;

        string txt = Mathf.RoundToInt(timer).ToString() + "s";
        timerText.text = txt;
    }

    public void MergeAmount(int amount)
    {
        timerAmount += amount;

        if (timerAmount >= 0)
        {
            numText.text = "+" + timerAmount.ToString() + "%";
            numText.color = Color.green;
        }
        else
        {
            numText.text = "-" + timerAmount.ToString() + "%";
            numText.color = Color.red;
        }
    }

    public void TimerEnd()
    {
        if (slotType == 8)
        {
            manager.perfectGuardEffect = PerfectGuardEffects.None;
            manager.perfectGuardValue = 0;
            manager.perfectGuardTeam = false;
        }

        if (slotType == 7)
        {
            if (manager.isFriendly)
            {
                manager.GM.battleManager.friendlyMonsterController.Stun(false, 0f);
            }
            else
            {
                manager.GM.battleManager.enemyMonsterController.Stun(false, 0f);
            }
        }

        if (slotType == 8)
        {
            if (manager.isFriendly)
            {
                manager.GM.battleManager.friendlyMonsterController.Guard(false, PerfectGuardEffects.None, 0f, false);
            }
            else
            {
                manager.GM.battleManager.enemyMonsterController.Guard(false, PerfectGuardEffects.None, 0f, false);
            }
        }


        active = false;
        manager.slotValues[slotType] = 0;
        manager.slots.Remove(this);
        
        Destroy(this.gameObject);
    }
}
