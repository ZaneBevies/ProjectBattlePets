using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FriendlyMonsterController : MonoBehaviour
{
    [Header("References")]
    public GameManager GM;
    public BattleHealthBar healthBar;
    public Animator friendlyAnim;
    public Animator friendlyAnimVariant;
    public Animator friendlyParentAnim;
    public SpriteRenderer friendlyDynamicSprite;
    public TextMeshProUGUI friendlyNameText;
    public TextMeshProUGUI friendlyLevelText;
    public FriendlyIconRotator friendlyIconRotator;
    public HitNumbers hitNumbers;
    public BattleBuffManager friendlyBattleBuffManager;

    public StunManager stunManager;
    public UIStunManager uiStunManager;

    public MoveController friendlyMoveController;
    [Header("Effects")]
    public SpriteRenderer guardRenderer;
    public Sprite blueGuard;
    public Sprite yellowGuard;
    [Header("Controller")]
    private Rigidbody2D rb;
    public Transform firePoint;

    public bool isGrounded;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;
    public float jumpForce;

    private bool doLand;

    [Header("Monster")]
    public Monster friendlyMonster;
    private List<Monster> backupMonsters;
    public int currentSlot;

    [Header("Stats and Cooldowns")]

    [HideInInspector]public int oomph, guts, juice, edge, wits, spark;

    public List<bool> basicReady = new List<bool>();
    public List<bool> specialReady = new List<bool>();
    public List<bool> tagReady = new List<bool>();


    public List<float> basicC = new List<float>();
    public List<float> specialC = new List<float>();
    public List<float> tagC = new List<float>();

    public List<bool> basicReady2 = new List<bool>();
    public List<bool> specialReady2 = new List<bool>();
    public List<bool> tagReady2 = new List<bool>();


    public List<float> basicC2 = new List<float>();
    public List<float> specialC2 = new List<float>();
    public List<float> tagC2 = new List<float>();

    public List<bool> basicReady3 = new List<bool>();
    public List<bool> specialReady3 = new List<bool>();
    public List<bool> tagReady3 = new List<bool>();
     

    public List<float> basicC3 = new List<float>();
    public List<float> specialC3 = new List<float>();
    public List<float> tagC3 = new List<float>();

    private int basicExtraCharges = 0;
    private int specialExtraCharges = 0;
    private int tagExtraCharges = 0;

    private bool regenOn = false;
    private float regenTimer = 0f;
    private float regenBaseTime = 1f;

    private bool regenCooldown = true;
    private float regenCooldownTime = 0f;
    private float regenCoolodownBaseTime = 3f;

    public List<float> inBattleTime = new List<float>();

    [HideInInspector]public List<GameObject> projectiles = new List<GameObject>();

    [HideInInspector] public bool alive = false;
    private bool tagOn = false;
    private float tagTimer = 0f;
    private int taggingSlot = 0;

    public bool guardOn = false;
    private float parryTimer = 0f;
    private bool parryOn = false;
    private PerfectGuardEffects perfectGuard;
    private float perfectValue;
    private bool perfectTeam = false;
    private bool critAttacks = false;
    private bool takingCrits = false;

    public MaskCutout maskCutout;


    public float airCritHeight = 1f;

    public bool airCrit = false;

    private bool invulnerable = false;
    private float invTime = 0f;

    private bool stunned = false;

    private void Start()
    {
       


        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (transform.position.y >= airCritHeight)
        {
            airCrit = true;
        }
        else
        {
            airCrit = false;
        }

        if (invulnerable)
        {
            if (invTime > 0)
            {
                invTime -= Time.deltaTime;
            }
            else if (invTime <= 0)
            {
                invTime = 0f;
                invulnerable = false;
            }
        }

        if (alive)
        {
            if (currentSlot == 0)
            {
                inBattleTime[0] += Time.deltaTime;
            }
            else if (currentSlot == 1)
            {
                inBattleTime[1] += Time.deltaTime;
            }
            else if (currentSlot == 2)
            {
                inBattleTime[2] += Time.deltaTime;
            }
        }
        


        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        if (isGrounded)
        {
            if (doLand)
            {
                StopJump();
                friendlyParentAnim.SetTrigger("Land");
                doLand = false;
            }
        }
        else
        {
            if (!doLand)
            {
                friendlyParentAnim.SetTrigger("Jump");
            }
            doLand = true;
        }

        DoCooldowns();


        if (regenCooldown)
        {
            if (regenCooldownTime > 0)
            {
                regenCooldownTime -= Time.deltaTime;
            }
            else if (regenCooldownTime <= 0)
            {
                //REGEN
                regenOn = true;
                regenCooldown = false;
            }
        }

        if (regenOn)
        {
            if (regenTimer > 0)
            {
                regenTimer -= Time.deltaTime;
            }
            else if (regenTimer <= 0)
            {
                //REGEN
                float regenAmount = 0.1f * (juice + (juice * ((friendlyBattleBuffManager.slotValues[2]) / 100)));

                if (regenAmount >= 1)
                {
                    healthBar.SetHealth(GM.playerHP + regenAmount);
                    GM.playerHP = healthBar.slider.value;
                }
                

                regenTimer = regenBaseTime;
            }
        }

        if (tagOn)
        {
            if (tagTimer > 0)
            {
                //Debug.Log("Hello");
                tagTimer -= Time.deltaTime;
            }
            else if (tagTimer <= 0)
            {
                GM.battleManager.ResumeControls();
                SpawnNewMonster(taggingSlot);
                tagTimer = 0f;
                tagOn = false;
                //transform.localPosition = new Vector3(-5f, -1f, 0f);
            }
        }

        if (parryOn)
        {
            if (parryTimer > 0)
            {
                parryTimer -= Time.deltaTime;
            }
            else if (parryTimer <= 0)
            {
                guardRenderer.sprite = blueGuard;
                parryOn = false;
                parryTimer = 0f;
            }
        }

        if (stunned)
        {
            healthBar.ChangeColor("Yellow");
        }
        else
        {
            if (guardOn)
            {
                healthBar.ChangeColor("Blue");
            }
            else
            {
                if (friendlyBattleBuffManager.slotValues[6] > 0)
                {
                    healthBar.ChangeColor("Red");
                }
                else
                {
                    float regenAmount = 0.1f * (juice + (juice * ((friendlyBattleBuffManager.slotValues[2]) / 100)));

                    if (regenAmount >= 1)
                    {
                        healthBar.ChangeColor("BrightGreen");
                    }
                    else
                    {
                        healthBar.ChangeColor("Normal");
                    }
                }
            }
        }
    }
    
    private void DoCooldowns()
    { // 1111111111
        for (int i = 0; i < basicReady.Count; i++)
        {
            if (!basicReady[i])
            {
                if (basicC[i] > 0) { basicC[i] -= Time.deltaTime; }
                else if (basicC[i] <= 0) { basicReady[i] = true; basicC[i] = 0f; }
            }
        }

        for (int i = 0; i < specialReady.Count; i++)
        {
            if (!specialReady[i])
            {
                if (specialC[i] > 0) { specialC[i] -= Time.deltaTime; }
                else if (specialC[i] <= 0) { specialReady[i] = true; specialC[i] = 0f; }
            }
        }

        for (int i = 0; i < tagReady.Count; i++)
        {
            if (!tagReady[i])
            {
                if (tagC[i] > 0) { tagC[i] -= Time.deltaTime; }
                else if (tagC[i] <= 0) { tagReady[i] = true; tagC[i] = 0f; }
            }
        }
        // 22222222222
        for (int i = 0; i < basicReady2.Count; i++)
        {
            if (!basicReady2[i] && basicReady[i])
            {
                if (basicC2[i] > 0) { basicC2[i] -= Time.deltaTime; }
                else if (basicC2[i] <= 0) { basicReady2[i] = true; basicC2[i] = 0f; }
            }
        }

        for (int i = 0; i < specialReady2.Count; i++)
        {
            if (!specialReady2[i] && specialReady[i])
            {
                if (specialC2[i] > 0) { specialC2[i] -= Time.deltaTime; }
                else if (specialC2[i] <= 0) { specialReady2[i] = true; specialC2[i] = 0f; }
            }
        }

        for (int i = 0; i < tagReady2.Count; i++)
        {
            if (!tagReady2[i] && tagReady[i])
            {
                if (tagC2[i] > 0) { tagC2[i] -= Time.deltaTime; }
                else if (tagC2[i] <= 0) { tagReady2[i] = true; tagC2[i] = 0f; }
            }
        }
        // 33333333333333333
        for (int i = 0; i < basicReady3.Count; i++)
        {
            if (!basicReady3[i] && basicReady[i] && basicReady2[i])
            {
                if (basicC3[i] > 0) { basicC3[i] -= Time.deltaTime; }
                else if (basicC3[i] <= 0) { basicReady3[i] = true; basicC3[i] = 0f; }
            }
        }

        for (int i = 0; i < specialReady3.Count; i++)
        {
            if (!specialReady3[i] && specialReady[i] && specialReady2[i])
            {
                if (specialC3[i] > 0) { specialC3[i] -= Time.deltaTime; }
                else if (specialC3[i] <= 0) { specialReady3[i] = true; specialC3[i] = 0f; }
            }
        }

        for (int i = 0; i < tagReady3.Count; i++)
        {
            if (!tagReady3[i] && tagReady[i] && tagReady2[i])
            {
                if (tagC3[i] > 0) { tagC3[i] -= Time.deltaTime; }
                else if (tagC3[i] <= 0) { tagReady3[i] = true; tagC3[i] = 0f; }
            }
        }
    }
    public void RefreshCooldowns()
    {
        for (int i = 0; i < basicReady.Count; i++)
        {
            basicReady[i] = true;
            specialReady[i] = true;
            tagReady[i] = true;

            basicC[i] = 0f;
            specialC[i] = 0f;
            tagC[i] = 0f;
        }
    }

    private void SpawnNewMonster(int slot)
    {
        float cooldown = 2f;
        tagC[currentSlot] = cooldown - (cooldown * (0.008f * (spark + (spark * ((friendlyBattleBuffManager.slotValues[5]) / 100))))); //tagCooldown;
        tagReady[currentSlot] = false;


        currentSlot = slot;
        List<Monster> pMons = new List<Monster>();

        for (int i = 0; i < GM.collectionManager.partySlots.Count; i++)
        {
            if (i != slot)
            {
                if (GM.collectionManager.partySlots[i].storedMonsterObject != null)
                {
                    //Debug.Log("backupMon" + i + " " + GM.collectionManager.partySlots[i].storedMonsterObject.GetComponent<Slot>().storedMonster.name);
                    pMons.Add(GM.collectionManager.partySlots[i].storedMonsterObject.GetComponent<Slot>().storedMonster);
                }

            }
        }

        GM.battleManager.slotSelected = slot + 1;

        friendlyMonster = GM.collectionManager.partySlots[slot].storedMonsterObject.GetComponent<Slot>().storedMonster;


        //SETS STATS VALUES OF THE CONTROLLER TO WHATEVERT IS SWITCHED TO
        oomph = Mathf.RoundToInt(friendlyMonster.stats[0].value * friendlyMonster.nature.addedStats[0].value);
        guts = Mathf.RoundToInt(friendlyMonster.stats[1].value * friendlyMonster.nature.addedStats[1].value);
        juice = Mathf.RoundToInt(friendlyMonster.stats[2].value * friendlyMonster.nature.addedStats[2].value);
        edge = Mathf.RoundToInt(friendlyMonster.stats[3].value * friendlyMonster.nature.addedStats[3].value);
        wits = Mathf.RoundToInt(friendlyMonster.stats[4].value * friendlyMonster.nature.addedStats[4].value);
        spark = Mathf.RoundToInt(friendlyMonster.stats[5].value * friendlyMonster.nature.addedStats[5].value);

        float edgeAmount = edge + friendlyBattleBuffManager.slotValues[3];
        float witsAmount = wits + friendlyBattleBuffManager.slotValues[4];
        float sparkAmount = spark + friendlyBattleBuffManager.slotValues[5];

        basicExtraCharges = 0;
        specialExtraCharges = 0;
        tagExtraCharges = 0;

        while (edgeAmount > 100)
        {
            edgeAmount -= 100f;
            basicExtraCharges++;
        }

        while (witsAmount > 100)
        {
            witsAmount -= 100f;
            specialExtraCharges++;
        }

        while (sparkAmount > 100)
        {
            sparkAmount -= 100f;
            tagExtraCharges++;
        }


        //tagC[currentSlot] = tagCooldown;
        //Debug.Log(friendlyMonster.name + pMons[0].name + pMons[1].name);
        backupMonsters = pMons;
        friendlyIconRotator.SetMonsterRotator(friendlyMonster, pMons); // HERE


        friendlyAnim.runtimeAnimatorController = friendlyMonster.animator;
        friendlyAnimVariant.runtimeAnimatorController = friendlyMonster.variant.variantAnimator;
        friendlyDynamicSprite.color = friendlyMonster.colour.colour;
        friendlyNameText.text = friendlyMonster.name;
        friendlyLevelText.text = "Level " + friendlyMonster.level.ToString();

    }
    public void SetFriendlyMonster(int slot)
    {
        if (GM.collectionManager.partySlots[slot].storedMonsterObject == null) return;

        if (!tagReady[slot]) { return; }


        if (!GM.battleManager.isPlayingIntro)
        {
            //Debug.Log("what?");
            GM.battleManager.PauseControls();

            taggingSlot = slot;
            tagTimer = 1f;
            maskCutout.Play(tagTimer);
            tagOn = true;
        }
        else
        {
            SpawnNewMonster(slot);
        }

        
    }

    public void TakeDamage(int damage, bool effect) // if effect cant be parried, guarded
    {
        //USE DEFENCE HERE

        if (parryOn)
        {
            // NO damage but do guard effect
            //Debug.Log("YO");
            if (perfectGuard == PerfectGuardEffects.CritAttack)
            {
                friendlyBattleBuffManager.AddBuff(perfectValue, 9);
            }
            else if (perfectGuard == PerfectGuardEffects.TakingCrits)
            {
                friendlyBattleBuffManager.AddBuff(perfectValue, 10);
            }
            else if (perfectGuard == PerfectGuardEffects.Stunned)
            {
                friendlyBattleBuffManager.AddBuff(perfectValue, 7);
            }
            else if (perfectGuard == PerfectGuardEffects.Heal)
            {
                Heal((int)perfectValue);
            }
            else if (perfectGuard == PerfectGuardEffects.Oomph)
            {
                if (perfectValue > 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Oomph, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Oomph, (int)perfectValue, perfectTeam);
                }
            }
            else if (perfectGuard == PerfectGuardEffects.Guts)
            {
                //Debug.Log("YO2");
                if (perfectValue > 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Guts, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Guts, (int)perfectValue, perfectTeam);
                }
            }
            else if (perfectGuard == PerfectGuardEffects.Juice)
            {
                if (perfectValue > 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Juice, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Juice, (int)perfectValue, perfectTeam);
                }
            }
            else if (perfectGuard == PerfectGuardEffects.Edge)
            {
                if (perfectValue > 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Edge, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Edge, (int)perfectValue, perfectTeam);
                }
            }
            else if (perfectGuard == PerfectGuardEffects.Wits)
            {
                if (perfectValue > 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Wits, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Wits, (int)perfectValue, perfectTeam);
                }
            }
            else if (perfectGuard == PerfectGuardEffects.Spark)
            {
                if (perfectValue > 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Spark, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    friendlyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Spark, (int)perfectValue, perfectTeam);
                }
            }

            hitNumbers.SpawnText("Parry", "Yellow");
            Guard(false, PerfectGuardEffects.None, 0f, false);
            
        }
        else if (effect || !guardOn && !invulnerable)
        {
            regenCooldownTime = regenCoolodownBaseTime;
            regenCooldown = true;

            regenOn = false;

            float flDmg;
            if (takingCrits)
            {
                flDmg = damage * 2;
            }
            else
            {
                flDmg = damage;
            }
            //Debug.Log("start dmg: " + flDmg);

            float gutsReal = 0f;
            float gutsAmount = guts + (guts * ((friendlyBattleBuffManager.slotValues[1]) / 100f));

            if (gutsAmount > 100)
            {
                gutsReal = 100f;

                float gutsNegateAmount = 0f;

                if (gutsAmount > 200)
                {
                    gutsNegateAmount = 100f;
                }
                else
                {
                    gutsNegateAmount = gutsAmount - 100f;
                }

                float rand = Random.Range(0f, 1f);
                float chance = ((0.008f * gutsNegateAmount));

                if (rand <= chance) // negate damage
                {

                    return;
                }
            }
            else
            {
                gutsReal = gutsAmount;
            }


            float trueDamage = flDmg - (flDmg * (0.008f * gutsReal));


            int dmg = Mathf.RoundToInt(trueDamage);
            int protectedDamage = dmg - (int)flDmg;
            //Debug.Log("True dmg: " + dmg);

            healthBar.SetHealth(GM.playerHP - dmg);
            GM.playerHP = (int)healthBar.slider.value;

            friendlyParentAnim.SetTrigger("Hit");



            if (GM.playerHP <= 0)
            {
                GM.battleManager.Lose();
            }
            else
            {
                if (dmg > 0)
                {
                    hitNumbers.SpawnDamageNumbersWithMod(dmg, protectedDamage, "Red", "Blue");
                }

            }
        }
        else
        {
            hitNumbers.SpawnText("Guarded", "Blue");
            Guard(false, PerfectGuardEffects.None, 0f, false);
        }
            
    }

    public void Heal(int amount)
    {
        if (GM.playerHP < 100)
        {
            int realAmount = amount;
            if (amount + GM.playerHP > 100)
            {
                realAmount = (amount + (int)GM.playerHP) - 100;
            }

            healthBar.SetHealth(GM.playerHP + amount);
            GM.playerHP = healthBar.slider.value;
            hitNumbers.SpawnDamageNumbersWithMod(realAmount, 0, "Green", "Blue");
        }
        
    }

    public void Stun(bool state, float time)
    {
        if (state)
        {
            stunned = true;
            GM.battleManager.PauseControls();
            uiStunManager.Stun(time);
            stunManager.Stun(time);
            GM.battleManager.captureButton.StopCharging();
        }
        else
        {
            stunned = false;
            GM.battleManager.ResumeControls();
            uiStunManager.StopStun();
            stunManager.StopStun();
        }
        
    }

    public void Guard(bool state, PerfectGuardEffects perfectGuardEffect, float perfectGuardValue, bool team)
    {
        if (state)
        {
            guardOn = true;
            guardRenderer.gameObject.SetActive(true);
            guardRenderer.sprite = yellowGuard;
            parryTimer = 0.5f;
            parryOn = true;

            perfectGuard = perfectGuardEffect;
            perfectValue = perfectGuardValue;
            perfectTeam = team;
        }
        else
        {
            guardOn = false;
            guardRenderer.gameObject.SetActive(false);
            parryTimer = 0f;
            parryOn = false;
            perfectGuard = PerfectGuardEffects.None;
            perfectValue = 0f;
            perfectTeam = false;
        }

    }

    public void CritAttacks(bool state)
    {
        critAttacks = state;
    }

    public void TakingCrits(bool state)
    {
        takingCrits = state;
    }

    public void Jump() // Jumps monster into air
    {
        rb.velocity = Vector2.up * jumpForce;

        friendlyAnim.SetBool("Jump", true);
        friendlyAnimVariant.SetBool("Jump", true);
    }

    public void StopJump()
    {
        friendlyAnim.SetBool("Jump", false);
        friendlyAnimVariant.SetBool("Jump", false);
    }

    public void Special() // Uses Special Attack
    {
        if (specialReady[currentSlot])
        {
            DoSpecial(0f, 100);
        }
        else
        {
            if (specialReady2[currentSlot] && specialExtraCharges > 0)
            {
                DoSpecial(100f, 200);
            }
            else
            {
                if (specialReady3[currentSlot] && specialExtraCharges > 1)
                {
                    DoSpecial(200f, 300);
                }
            }
        }

    }

    public void Attack() // Used Basic Attack
    {
        if (basicReady[currentSlot])
        {
            DoAttack(0f, 100);
        }
        else
        {
            if (basicReady2[currentSlot] && basicExtraCharges > 0)
            {
                DoAttack(100f, 200);
            }
            else
            {
                if (basicReady3[currentSlot] && basicExtraCharges > 1)
                {
                    DoAttack(200f, 300);
                }
            }
        }
        
    }
    private void DoSpecial(float sizeNum, int num)
    {
        if (friendlyMonster.specialMove.moveActions.Count > 0)
        {
            // use moves
            friendlyMoveController.UseMove(friendlyMonster.specialMove);
        }


        friendlyParentAnim.SetTrigger("Attack");
        friendlyAnim.SetTrigger("Special");
        friendlyAnimVariant.SetTrigger("Special");

        float valueAmount = wits + (wits * ((friendlyBattleBuffManager.slotValues[4]) / 100));
        float valueReal = 0f;


        if (valueAmount > num)
        {
            valueReal = 100f;

        }
        else
        {
            valueReal = valueAmount - sizeNum;
        }

        if (num == 100)
        {
            specialC[currentSlot] = friendlyMonster.specialMove.baseCooldown - (friendlyMonster.specialMove.baseCooldown * (0.008f * valueReal));
            specialReady[currentSlot] = false;
        }
        else if (num == 200)
        {
            specialC2[currentSlot] = friendlyMonster.specialMove.baseCooldown - (friendlyMonster.specialMove.baseCooldown * (0.008f * valueReal));
            specialReady2[currentSlot] = false;
        }
        else if (num == 300)
        {
            specialC3[currentSlot] = friendlyMonster.specialMove.baseCooldown - (friendlyMonster.specialMove.baseCooldown * (0.008f * valueReal));
            specialReady3[currentSlot] = false;
        }


    }
    private void DoAttack(float sizeNum, int num)
    {
        if (friendlyMonster.basicMove.moveActions.Count > 0)
        {
            friendlyMoveController.UseMove(friendlyMonster.basicMove);
        }
        friendlyParentAnim.SetTrigger("Attack");
        friendlyAnim.SetTrigger("Basic");
        friendlyAnimVariant.SetTrigger("Basic");

        float valueAmount = edge + (edge * ((friendlyBattleBuffManager.slotValues[3]) / 100));
        float valueReal = 0f;


        if (valueAmount > num)
        {
            valueReal = 100f;

        }
        else
        {
            valueReal = valueAmount - sizeNum;
        }

        if (num == 100)
        {
            basicC[currentSlot] = friendlyMonster.basicMove.baseCooldown - (friendlyMonster.basicMove.baseCooldown * (0.008f * valueReal));
            basicReady[currentSlot] = false;
        }
        else if (num == 200)
        {
            basicC2[currentSlot] = friendlyMonster.basicMove.baseCooldown - (friendlyMonster.basicMove.baseCooldown * (0.008f * valueReal));
            basicReady2[currentSlot] = false;
        }
        else if (num == 300)
        {
            basicC3[currentSlot] = friendlyMonster.basicMove.baseCooldown - (friendlyMonster.basicMove.baseCooldown * (0.008f * valueReal));
            basicReady3[currentSlot] = false;
        }


        invulnerable = true;
        invTime = 0.1f;

        
    }

    public void FireProjectile(GameObject prefab, float speed, int dmg, float lifeTime, int collideWithAmountOfObjects, bool criticalProjectile)
    {
        GameObject proj = Instantiate(prefab, firePoint.position, firePoint.rotation);
        projectiles.Add(proj);
        //Debug.Log("What?");
        if (critAttacks)
        {
            proj.GetComponent<Projectile>().Init(speed, dmg, lifeTime, collideWithAmountOfObjects, critAttacks, "Enemy", GM.battleManager.enemyMonsterController.enemyBattleBuffManager);
        }
        else
        {
            proj.GetComponent<Projectile>().Init(speed, dmg, lifeTime, collideWithAmountOfObjects, criticalProjectile, "Enemy", GM.battleManager.enemyMonsterController.enemyBattleBuffManager);
        }
        
    }
}
