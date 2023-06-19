using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleType
{
    Battle,
    Punk,
    ChallengeCrawl
}

public class BattleManager : MonoBehaviour
{
    [Header("References")]
    public GameManager GM;
    public IntroBattleTimelineController battleIntroTimeline;
    public GameObject fightingIntroObject;
    public SpriteRenderer backgroundImage;

    [Header("Friendly")]
    public FriendlyMonsterController friendlyMonsterController;

    public int slotSelected;

    public CaptureButton captureButton;

    [Header("Enemy")]
    public EnemyMonsterController enemyMonsterController;

    [Header("XP/Values")]
    public int xpGainedPerLevel = 20;

    public List<StoredItem> rewardedItems = new List<StoredItem>();

    [Header("TESTING")]
    public List<MonsterSpawn> testMonster;


    private int groupXp = 0;

    public bool controlsActive = true;

    public bool isPlayingIntro = true;

    public float enemyCapturePoints = 100f;

    //public float extraCaptureTimeEnemyLevel = 0f;
    //public float extraCaptureTimeMissingHealth = 0f;

    void Start()
    {
        // FOR TESTING 
        //InitBattle(testMonster, false);
      
    }


    public void InitBattle(List<MonsterSpawn> mons, NodeType type, Sprite background, List<StoredItem> rewardItems)
    {
        rewardedItems = rewardItems;
        isPlayingIntro = true;
        // Sets up the battle.
        // Generates the monster to fight
        // Sets the initial player monster to be switch in to first found going from top slot to bottom
        // Sets the BATTLE button to true and positons in the middle of the screen

        friendlyMonsterController.inBattleTime = new List<float>();
        friendlyMonsterController.inBattleTime.Add(0f);
        friendlyMonsterController.inBattleTime.Add(0f);
        friendlyMonsterController.inBattleTime.Add(0f);

        GM.battleUI.gameObject.SetActive(true);
        GM.battleGameobject.SetActive(true);
        backgroundImage.sprite = background;
        enemyMonsterController.RefreshCooldowns();
        friendlyMonsterController.RefreshCooldowns();

        enemyMonsterController.SetupEnemy(mons, type);
        friendlyMonsterController.healthBar.SetMaxHealth(100);
        friendlyMonsterController.healthBar.SetHealth(GM.playerHP);

        List<PassiveSO> passivesFriend = new List<PassiveSO>();
        for (int i = 0; i < GM.collectionManager.partySlots.Count; i++)
        {
            if (GM.collectionManager.partySlots[i].storedMonsterObject != null)
            {
                passivesFriend.Add(GM.collectionManager.partySlots[i].storedMonsterObject.GetComponent<Slot>().storedMonster.passiveMove);
            }
        }

        List<PassiveSO> passivesEnemy = new List<PassiveSO>();
        passivesEnemy.Add(GM.battleManager.enemyMonsterController.currentMonster.passiveMove);

        friendlyMonsterController.friendlyBattleBuffManager.StartPassives(passivesFriend);
        enemyMonsterController.enemyBattleBuffManager.StartPassives(passivesEnemy);

        for (int i = 0; i < 3; i++)
        {
            if (GM.collectionManager.partySlots[i].storedMonsterObject != null)
            {
                friendlyMonsterController.SetFriendlyMonster(i);
                break;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (GM.collectionManager.partySlots[i].storedMonsterObject != null)
            {
                if (i == 0)
                {
                    GM.battleUI.tagSprites1.tagSprites.UpdateArt(GM.collectionManager.partySlots[0].storedMonsterObject.GetComponent<PartySlot>().storedMonster);
                }
                else if (i == 1)
                {
                    GM.battleUI.tagSprites2.tagSprites.UpdateArt(GM.collectionManager.partySlots[1].storedMonsterObject.GetComponent<PartySlot>().storedMonster);
                }
                else if (i == 2)
                {
                    GM.battleUI.tagSprites3.tagSprites.UpdateArt(GM.collectionManager.partySlots[2].storedMonsterObject.GetComponent<PartySlot>().storedMonster);
                }
            }
        }

        


        //PLAY ENTER ANIM HERE
        fightingIntroObject.SetActive(true);

        battleIntroTimeline.Play(enemyMonsterController.currentMonster);


        
    }

    public void EnterStart()
    {
        //Debug.Log("Start");
        isPlayingIntro = true;
        PauseControls();

        friendlyMonsterController.alive = false;
        enemyMonsterController.ActivateAI(false);
    }

    public void EnterDone()
    {
        //Debug.Log("Start");
        isPlayingIntro = false;
        ResumeControls();

        friendlyMonsterController.alive = true;
        enemyMonsterController.ActivateAI(true);

        float averageLevel = 0f;
        int numOfMons = 0;

        for (int i = 0; i < 3; i++)
        {
            if (GM.collectionManager.partySlots[i].storedMonsterObject != null)
            {
                averageLevel += GM.collectionManager.partySlots[i].storedMonsterObject.GetComponent<PartySlot>().storedMonster.level;
                numOfMons++;
            }
        }

        averageLevel = averageLevel / numOfMons;


        float averageLevelEnemy = enemyMonsterController.currentMonster.level;
        int numOfMonsEnemy = 1;

        for (int i = 0; i < enemyMonsterController.backupMonsters.Count; i++)
        {
            averageLevelEnemy += enemyMonsterController.backupMonsters[i].level;
            numOfMonsEnemy++;
        }

        averageLevelEnemy = averageLevelEnemy / numOfMonsEnemy;

        float enemyDifference = averageLevelEnemy - averageLevel;

        if (enemyDifference < 1)
        {
            enemyCapturePoints = 100f;
        }
        else
        {
            enemyCapturePoints = 100 + (enemyDifference * 40f);
        }
        //Debug.Log(averageLevelEnemy);
        //Debug.Log(enemyDifference);
        //Debug.Log(enemyCapturePoints);

        enemyMonsterController.capBar.SetMaxCapturePoints(enemyCapturePoints, enemyMonsterController.enemyHealth);
        enemyMonsterController.capBar.SetCaptureLevel(0f, enemyMonsterController.enemyHealth);
    }


    

    // BUTTONS

    public void Switch(int slot) // switchs monster to said slotted monster in party IF possible
    {
        if (controlsActive && friendlyMonsterController.tagReady[slot] && friendlyMonsterController.currentSlot != slot)
        {
            friendlyMonsterController.SetFriendlyMonster(slot);
        }
        

    }

    public void GetXP()
    {
        int baseXp = xpGainedPerLevel;
        for (int i = 0; i < enemyMonsterController.currentMonster.level - 1; i++)
        {
            baseXp = Mathf.RoundToInt(baseXp * 1.2f);
        }

        groupXp = groupXp + baseXp;
    }

    public void Win()
    {
        friendlyMonsterController.alive = false;
        CleanUpProjectiles();
        enemyMonsterController.ActivateAI(false);

        GM.playerManager.currentNode.SetComplete(true);
        GM.playerManager.currentNode.Refresh();

        GM.battleGameobject.SetActive(false);
        GM.battleUI.gameObject.SetActive(false);

        GM.overworldGameobject.SetActive(true);
        GM.overworldUI.gameObject.SetActive(true);

        int num = 0;
        for (int i = 0; i < 3; i++)
        {
            if (GM.collectionManager.partySlots[i].storedMonsterObject != null)
            {
                num++;
            }
        }


        GM.DoBattleAftermath("WON", friendlyMonsterController.inBattleTime, groupXp, num);
        groupXp = 0;

        GM.SaveData();
    }

    public void Lose()
    {
        friendlyMonsterController.alive = false;
        CleanUpProjectiles();
        enemyMonsterController.ActivateAI(false);

        GM.battleGameobject.SetActive(false);
        GM.battleUI.gameObject.SetActive(false);

        GM.overworldGameobject.SetActive(true);
        GM.overworldUI.gameObject.SetActive(true);

        int num = 0;
        for (int i = 0; i < 3; i++)
        {
            if (GM.collectionManager.partySlots[i].storedMonsterObject != null)
            {
                num++;
            }
        }

        GM.DoBattleAftermath("LOST", friendlyMonsterController.inBattleTime, 0, num);
        groupXp = 0;
    }


    public void Capture() // Captures the monster
    {
        //PAUSE GAME
        enemyMonsterController.ActivateAI(false);
        GM.captureChoiceWindow.Init(enemyMonsterController.currentMonster, enemyMonsterController);
        CleanUpProjectiles();

    }

    public void Special() // Uses Special Attack
    {
        if (controlsActive)
        {
            friendlyMonsterController.Special();
        }
        
    }

    public void Attack() // Used Basic Attack
    {
        if (controlsActive)
        {
            friendlyMonsterController.Attack();
        }
        
    }

    public void PauseControls()
    {
        GM.battleUI.DisableControls();
        controlsActive = false;
    }

    public void PauseControlsExceptCapture()
    {
        GM.battleUI.DisableAllButCapture();
        controlsActive = false;
    }


    public void ResumeControls()
    {
        GM.battleUI.EnableControls();
        controlsActive = true;
    }

    private void CleanUpProjectiles()
    {
        for (int i = 0; i < friendlyMonsterController.projectiles.Count; i++)
        {
            Destroy(friendlyMonsterController.projectiles[i].gameObject);
        }

        for (int i = 0; i < enemyMonsterController.projectiles.Count; i++)
        {
            Destroy(enemyMonsterController.projectiles[i].gameObject);
        }

        friendlyMonsterController.projectiles = new List<GameObject>();
        enemyMonsterController.projectiles = new List<GameObject>();

        friendlyMonsterController.hitNumbers.End();
        enemyMonsterController.hitNumbers.End();

        friendlyMonsterController.friendlyBattleBuffManager.ClearSlotsAfterBattle();
        enemyMonsterController.enemyBattleBuffManager.ClearSlotsAfterBattle();

        friendlyMonsterController.stunManager.StopStun();
        enemyMonsterController.stunManager.StopStun();

        GM.battleUI.captureBute.ResetBar();

    }
}
