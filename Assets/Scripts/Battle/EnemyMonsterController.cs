using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EnemyMonsterController : MonoBehaviour
{
    public float enemyHealth = 100;

    [Header("References")]
    public GameManager GM;
    public MonsterAIController aiController;
    public BattleHealthBar healthBar;
    public BattleCapBar capBar;
    public Animator enemyAnim;
    public Animator enemyAnimVariant;
    public Animator enemyParentAnim;
    public SpriteRenderer enemyDynamicSprite;
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyLevelText;
    public EnemyIconRotator enemyIconRotator;
    public HitNumbers hitNumbers;
    public BattleBuffManager enemyBattleBuffManager;

    public StunManager stunManager;

    public MoveController enemyMoveController;

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
    //public float jumpForce;

    private bool doLand;
    
    
    [Header("Monster")]
    public Monster currentMonster;
    [HideInInspector] public List<Monster> backupMonsters = new List<Monster>();
    public int currentSlot;

    [Header("Stats and Cooldowns")]

    [HideInInspector] public int oomph, guts, juice, edge, wits, spark;

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

    [HideInInspector] public List<GameObject> projectiles = new List<GameObject>();

    private bool tagOn = false;
    private float tagTimer = 0f;

    public MaskCutout maskCutout;

    public bool guardOn = false;
    private float parryTimer = 0f;
    private bool parryOn = false;
    private PerfectGuardEffects perfectGuard;
    private float perfectValue;
    private bool perfectTeam = false;

    private bool critAttacks = false;
    private bool takingCrits = false;

    public float airCritHeight = 1f;
    public bool airCrit = false;

    private bool invulnerable = false;
    private float invTime = 0f;

    private bool stunned = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
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

        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);

        if (isGrounded)
        {
            if (doLand)
            {
                StopJump();
                enemyParentAnim.SetTrigger("Land");
                doLand = false;
            }
        }
        else
        {
            if (!doLand)
            {
                enemyParentAnim.SetTrigger("Jump");
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
                float regenAmount = 0.1f * (juice + (juice * ((enemyBattleBuffManager.slotValues[2]) / 100)));

                healthBar.SetHealth(enemyHealth + regenAmount);
                enemyHealth = healthBar.slider.value;

                //capBar.SetCaptureLevel(GM.battleManager.enemyCapturePoints, enemyHealth);

                regenTimer = regenBaseTime;
            }
        }

        if (tagOn) 
        {
            if (tagTimer > 0)
            {
                tagTimer -= Time.deltaTime;
                //transform.localPosition = Vector3.Lerp(transform.position, new Vector3(10f, -1f, 0f), tagTimer / 1f);
            }
            else if (tagTimer <= 0)
            {
                aiController.SetActive(true);
                tagOn = false;
                SpawnNewMonster();
                //transform.localPosition = new Vector3(5f, -1f, 0f);
                tagTimer = 0f;
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
                if (enemyBattleBuffManager.slotValues[6] > 0)
                {
                    healthBar.ChangeColor("Red");
                }
                else
                {
                    float regenAmount = 0.1f * (juice + (juice * ((enemyBattleBuffManager.slotValues[2]) / 100)));

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

    

    public void SetupEnemy(List<MonsterSpawn> mons, NodeType type)
    {
        for (int i = 0; i < mons.Count; i++)
        {
            backupMonsters.Add(new Monster(Random.Range(mons[i].minLevel, mons[i].maxLevel), mons[i].monster));
        }

        currentSlot = -1;

        SetMonsterActive();

        if (type == NodeType.Punk)
        {
            aiController.SetBattleType(true);
        }

        
        
    }
    private void SpawnNewMonster()
    {
        currentMonster = backupMonsters[0];
        currentSlot++;

        enemyHealth = 100;
        healthBar.SetMaxHealth(100);
        healthBar.SetHealth(100);

        //capBar.SetMaxCapturePoints(GM.battleManager.enemyCapturePoints, enemyHealth);
        //capBar.SetCaptureLevel(0f, enemyHealth);

        backupMonsters.RemoveAt(0);

        

        //SETS STATS VALUES OF THE CONTROLLER TO WHATEVERT IS SWITCHED TO
        float oomphA = 0, oomphB = 0;
        float gutsA = 0, gutsB = 0;
        float juiceA = 0, juiceB = 0;
        float edgeA = 0, edgeB = 0;
        float witsA = 0, witsB = 0;
        float sparkA = 0, sparkB = 0;


        if (currentMonster.level - 1 <= 20)
        {
            oomphA = (1 * (currentMonster.stats[0].value / 5)) * (currentMonster.level - 1);
            oomphB = 0;

            gutsA = (1 * (currentMonster.stats[1].value / 5)) * (currentMonster.level - 1);
            gutsB = 0;

            juiceA = (1 * (currentMonster.stats[2].value / 5)) * (currentMonster.level - 1);
            juiceB = 0;

            edgeA = (1 * (currentMonster.stats[3].value / 5)) * (currentMonster.level - 1);
            edgeB = 0;

            witsA = (1 * (currentMonster.stats[4].value / 5)) * (currentMonster.level - 1);
            witsB = 0;

            sparkA = (1 * (currentMonster.stats[5].value / 5)) * (currentMonster.level - 1);
            sparkB = 0;
        }
        else if (currentMonster.level - 1 > 20 && currentMonster.level - 1 <= 40)
        {
            oomphA = (1 * (currentMonster.stats[0].value / 5)) * 19;
            oomphB = (0.66f * (currentMonster.stats[0].value / 5)) * (currentMonster.level - 20);

            gutsA = (1 * (currentMonster.stats[1].value / 5)) * 19;
            gutsB = (0.66f * (currentMonster.stats[1].value / 5)) * (currentMonster.level - 20);

            juiceA = (1 * (currentMonster.stats[2].value / 5)) * 19;
            juiceB = (0.66f * (currentMonster.stats[2].value / 5)) * (currentMonster.level - 20);

            edgeA = (1 * (currentMonster.stats[3].value / 5)) * 19;
            edgeB = (0.66f * (currentMonster.stats[3].value / 5)) * (currentMonster.level - 20);

            witsA = (1 * (currentMonster.stats[4].value / 5)) * 19;
            witsB = (0.66f * (currentMonster.stats[4].value / 5)) * (currentMonster.level - 20);

            sparkA = (1 * (currentMonster.stats[5].value / 5)) * 19;
            sparkB = (0.66f * (currentMonster.stats[5].value / 5)) * (currentMonster.level - 20);
        }


        oomph = Mathf.FloorToInt((oomphA + oomphB + currentMonster.stats[0].value) * currentMonster.nature.addedStats[0].value);
        guts = Mathf.FloorToInt((gutsA + gutsB + currentMonster.stats[1].value) * currentMonster.nature.addedStats[1].value);
        juice = Mathf.FloorToInt((juiceA + juiceB + currentMonster.stats[2].value) * currentMonster.nature.addedStats[2].value);
        edge = Mathf.FloorToInt((edgeA + edgeB + currentMonster.stats[3].value) * currentMonster.nature.addedStats[3].value);
        wits = Mathf.FloorToInt((witsA + witsB + currentMonster.stats[4].value) * currentMonster.nature.addedStats[4].value);
        spark = Mathf.FloorToInt((sparkA + sparkB + currentMonster.stats[5].value) * currentMonster.nature.addedStats[5].value);

        float edgeAmount = edge + enemyBattleBuffManager.slotValues[3];
        float witsAmount = wits + enemyBattleBuffManager.slotValues[4];
        float sparkAmount = spark + enemyBattleBuffManager.slotValues[5];

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

        //oomph = currentMonster.stats[0].value + currentMonster.nature.addedStats[0].value;
        //guts = currentMonster.stats[1].value + currentMonster.nature.addedStats[1].value;
        //juice = currentMonster.stats[2].value + currentMonster.nature.addedStats[2].value;
        //edge = currentMonster.stats[3].value + currentMonster.nature.addedStats[3].value;
        //wits = currentMonster.stats[4].value + currentMonster.nature.addedStats[4].value;
        //spark = currentMonster.stats[5].value + currentMonster.nature.addedStats[5].value;


        //Debug.Log(friendlyMonster.name + pMons[0].name + pMons[1].name);
        enemyIconRotator.SetMonsterRotator(currentMonster, backupMonsters); // HERE

        enemyAnim.runtimeAnimatorController = currentMonster.animator;
        enemyAnimVariant.runtimeAnimatorController = currentMonster.variant.variantAnimator;
        enemyDynamicSprite.color = currentMonster.colour.colour;
        enemyNameText.text = currentMonster.name;
        enemyLevelText.text = "Level " + currentMonster.level.ToString();

    }
    public void SetMonsterActive()
    {
        if (backupMonsters.Count <= 0) { return; }

        if (!GM.battleManager.isPlayingIntro)
        {
            aiController.SetActive(false);
            tagTimer = 1f;
            maskCutout.Play(tagTimer);
            tagOn = true;
        }
        else
        {
            SpawnNewMonster();
        }

        

        
    }

    public void ActivateAI(bool state)
    {
        aiController.SetActive(state);
        transform.position = new Vector3(5f, -1f, 0f);
    }


    public void TakeDamage(int damage, bool effect)
    {
        if (parryOn)
        {
            // NO damage but do guard effect
            if (perfectGuard == PerfectGuardEffects.CritAttack)
            {
                enemyBattleBuffManager.AddBuff(perfectValue, 9);
            }
            else if (perfectGuard == PerfectGuardEffects.TakingCrits)
            {
                enemyBattleBuffManager.AddBuff(perfectValue, 10);
            }
            else if (perfectGuard == PerfectGuardEffects.Stunned)
            {
                enemyBattleBuffManager.AddBuff(perfectValue, 7);
            }
            else if (perfectGuard == PerfectGuardEffects.Heal)
            {
                Heal((int)perfectValue);
            }
            else if (perfectGuard == PerfectGuardEffects.Oomph)
            {
                if (perfectValue > 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Oomph, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Oomph, (int)perfectValue, perfectTeam);
                }
            }
            else if (perfectGuard == PerfectGuardEffects.Guts)
            {
                if (perfectValue > 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Guts, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Guts, (int)perfectValue, perfectTeam);
                }
            }
            else if (perfectGuard == PerfectGuardEffects.Juice)
            {
                if (perfectValue > 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Juice, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Juice, (int)perfectValue, perfectTeam);
                }
            }
            else if (perfectGuard == PerfectGuardEffects.Edge)
            {
                if (perfectValue > 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Edge, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Edge, (int)perfectValue, perfectTeam);
                }
            }
            else if (perfectGuard == PerfectGuardEffects.Wits)
            {
                if (perfectValue > 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Wits, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Wits, (int)perfectValue, perfectTeam);
                }
            }
            else if (perfectGuard == PerfectGuardEffects.Spark)
            {
                if (perfectValue > 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Buff, EffectedStat.Spark, (int)perfectValue, perfectTeam);
                }
                else if (perfectValue < 0)
                {
                    enemyBattleBuffManager.AddBuff(StatModType.Debuff, EffectedStat.Spark, (int)perfectValue, perfectTeam);
                }
            }
            hitNumbers.SpawnText("Parry", "Yellow");
            Guard(false, PerfectGuardEffects.None, 0f, false);
        }    
        else if (effect || !guardOn  && !invulnerable)
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
            float gutsAmount = guts + (guts * (enemyBattleBuffManager.slotValues[1] / 100f));
           // Debug.Log("Dmg : " + flDmg);
            


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

            //Debug.Log("Real Guts: " + gutsReal);
            //Debug.Log("True dmg: " + dmg);
            //Debug.Log("Protected dmg: " + protectedDamage);

            healthBar.SetHealth(enemyHealth - dmg);
            enemyHealth = healthBar.slider.value;

            enemyParentAnim.SetTrigger("Hit");

            

            if (enemyHealth <= 0)
            {
                if (backupMonsters.Count > 0)
                {
                    GM.battleManager.GetXP();
                    SetMonsterActive();
                }
                else
                {
                    GM.battleManager.GetXP();
                    GM.battleManager.Win();
                }

            }
            else
            {
                if (dmg > 0)
                {
                    hitNumbers.SpawnDamageNumbersWithMod(dmg, protectedDamage, "Red", "Blue");
                }

                
            }
        }
        else // STUPID LONG CODE FOR GUARD STUFF
        {
            hitNumbers.SpawnText("Guarded", "Blue");
            Guard(false, PerfectGuardEffects.None, 0f, false);
        }
    }

    public void Heal(int amount)
    {
        if (enemyHealth < 100)
        {
            int realAmount = amount;
            if (amount + enemyHealth > 100)
            {
                realAmount = (amount + (int)enemyHealth) - 100;
            }

            healthBar.SetHealth(enemyHealth + amount);
            enemyHealth = healthBar.slider.value;
            hitNumbers.SpawnDamageNumbersWithMod(realAmount, 0, "Green", "Blue");
        }
    }

    public void Stun(bool state, float time)
    {
        if (state)
        {
            stunned = true;
            ActivateAI(false);
            stunManager.Stun(time);
        }
        else
        {
            stunned = false;
            ActivateAI(true);
            stunManager.Stun(time);
        }

    }

    public void Guard(bool state, PerfectGuardEffects perfectGuardEffect, float perfectGuardValue, bool team)
    {
        //Debug.Log("guarding");
        if (state)
        {
            //Debug.Log("true");
            guardOn = true;
            guardRenderer.gameObject.SetActive(true);
            guardRenderer.sprite = yellowGuard;
            parryTimer = 1f;
            parryOn = true;
            perfectGuard = perfectGuardEffect;
            perfectValue = perfectGuardValue;
            perfectTeam = team;
        }
        else
        {
            //Debug.Log("false");
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


    public void Jump(int height) // Jumps monster into air
    {
        if (!isGrounded) return;

        rb.velocity = Vector2.up  * height;

        enemyAnim.SetBool("Jump", true);
        enemyAnimVariant.SetBool("Jump", true);
    }

    public void StopJump()
    {
        enemyAnim.SetBool("Jump", false);
        enemyAnimVariant.SetBool("Jump", false);
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
        if (currentMonster.specialMove.moveActions.Count > 0)
        {
            // use moves
            enemyMoveController.UseMove(currentMonster.specialMove);
        }


        enemyParentAnim.SetTrigger("Attack");
        enemyAnim.SetTrigger("Special");
        enemyAnimVariant.SetTrigger("Special");

        float valueAmount = wits + (wits * ((enemyBattleBuffManager.slotValues[4]) / 100));
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
            specialC[currentSlot] = currentMonster.specialMove.baseCooldown - (currentMonster.specialMove.baseCooldown * (0.008f * valueReal));
            specialReady[currentSlot] = false;
        }
        else if (num == 200)
        {
            specialC2[currentSlot] = currentMonster.specialMove.baseCooldown - (currentMonster.specialMove.baseCooldown * (0.008f * valueReal));
            specialReady2[currentSlot] = false;
        }
        else if (num == 300)
        {
            specialC3[currentSlot] = currentMonster.specialMove.baseCooldown - (currentMonster.specialMove.baseCooldown * (0.008f * valueReal));
            specialReady3[currentSlot] = false;
        }


    }
    private void DoAttack(float sizeNum, int num)
    {
        if (currentMonster.basicMove.moveActions.Count > 0)
        {
            enemyMoveController.UseMove(currentMonster.basicMove);
        }
        enemyParentAnim.SetTrigger("Attack");
        enemyAnim.SetTrigger("Basic");
        enemyAnimVariant.SetTrigger("Basic");

        float valueAmount = edge + (edge * ((enemyBattleBuffManager.slotValues[3]) / 100));
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
            basicC[currentSlot] = currentMonster.basicMove.baseCooldown - (currentMonster.basicMove.baseCooldown * (0.008f * valueReal));
            basicReady[currentSlot] = false;
        }
        else if (num == 200)
        {
            basicC2[currentSlot] = currentMonster.basicMove.baseCooldown - (currentMonster.basicMove.baseCooldown * (0.008f * valueReal));
            basicReady2[currentSlot] = false;
        }
        else if (num == 300)
        {
            basicC3[currentSlot] = currentMonster.basicMove.baseCooldown - (currentMonster.basicMove.baseCooldown * (0.008f * valueReal));
            basicReady3[currentSlot] = false;
        }

        invulnerable = true;
        invTime = 0.1f;
    }

    public void FireProjectile(GameObject prefab, float speed, int dmg, float lifeTime, int collideWithAmountOfObjects, bool criticalProjectile)
    {
        

        GameObject proj = Instantiate(prefab, firePoint.position, firePoint.rotation);
        projectiles.Add(proj);

        if (critAttacks)
        {
            proj.GetComponent<Projectile>().Init(speed, dmg, lifeTime, collideWithAmountOfObjects, critAttacks, "Friend", GM.battleManager.friendlyMonsterController.friendlyBattleBuffManager);
        }
        else
        {
            proj.GetComponent<Projectile>().Init(speed, dmg, lifeTime, collideWithAmountOfObjects, criticalProjectile, "Friend", GM.battleManager.friendlyMonsterController.friendlyBattleBuffManager);
        }

        
    }
}
