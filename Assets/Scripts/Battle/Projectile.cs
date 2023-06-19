using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int damage;

    public GameObject impactEffect;
    public Rigidbody2D rb;
    public SpriteRenderer sprite;

    public string opposition;


    public List<int> statsBuff = new List<int>();

    public float critAttackTime = 0f;
    public float takingCritsTime = 0f;
    public float stunnedTime = 0f;

    public int dotAmount = 0;
    public float dotTime = 0f;

    public bool resetSpecialCooldownOnHit = false;


    private bool life = false;
    private float lifeTimeTimer = 0f;

    private int amountOfObjectsToCollideWith = 0; // 0 means it will ignore other projectiles, 1 will break 1 and explode, 2 will break 1 and then the second its breaks dies with it
    private bool criticalProj = false;

    private BattleBuffManager manager;
    void Update()
    {
        if (life)
        {
            if (lifeTimeTimer > 0)
            {
                lifeTimeTimer -= Time.deltaTime;
            }
            else if (lifeTimeTimer <= 0)
            {
                lifeTimeTimer = 0f;
                life = false;
                Instantiate(impactEffect, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }
    }

    public void Init(float spd, int dmg, float lifeTime, int collideWithAmountOfObjects, bool criticalProjectile, string op, BattleBuffManager man)
    {
        manager = man;
        if (op == "Enemy") // opposition is
        {
            sprite.flipX = false;
            rb.velocity = transform.right * spd;
        }
        else if (op == "Friend") // opposition is
        {
            sprite.flipX = true;
            rb.velocity = -transform.right * spd;
        }

        
        damage = dmg;

        amountOfObjectsToCollideWith = collideWithAmountOfObjects;
        criticalProj = criticalProjectile;



        opposition = op;

        if (lifeTime > 0)
        {
            lifeTimeTimer = lifeTime;
            life = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (opposition == "Enemy")
        {
            EnemyMonsterController enemy = collision.GetComponent<EnemyMonsterController>();
            if (enemy != null)
            {
                if (!enemy.guardOn)
                {

                    if (dotAmount > 0 || dotTime > 0) // has dot
                    {
                        manager.AddBuff(dotAmount, dotTime);
                    }

                    if (critAttackTime > 0)
                    {
                        manager.AddBuff(critAttackTime, 1);
                    }

                    if (takingCritsTime > 0)
                    {
                        manager.AddBuff(takingCritsTime, 2);
                    }

                    if (stunnedTime > 0)
                    {
                        manager.AddBuff(stunnedTime, 3);
                    }

                    if (resetSpecialCooldownOnHit)
                    {
                        manager.GM.battleManager.friendlyMonsterController.specialReady[manager.GM.battleManager.friendlyMonsterController.currentSlot] = true;
                        manager.GM.battleManager.friendlyMonsterController.specialC[manager.GM.battleManager.friendlyMonsterController.currentSlot] = 0f;
                    }
                }

                if (criticalProj) // has crit
                {
                    enemy.TakeDamage(damage * 2, false);
                }
                else
                {
                    enemy.TakeDamage(damage, false);
                }


                Instantiate(impactEffect, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }

        if (opposition == "Friend")
        {
            FriendlyMonsterController friend = collision.GetComponent<FriendlyMonsterController>();
            if (friend != null)
            {
                

                if (!friend.guardOn)
                {
                    if (dotAmount > 0 || dotTime > 0) // has dot
                    {
                        manager.AddBuff(dotAmount, dotTime);
                    }

                    if (critAttackTime > 0)
                    {
                        manager.AddBuff(critAttackTime, 1);
                    }

                    if (takingCritsTime > 0)
                    {
                        manager.AddBuff(takingCritsTime, 2);
                    }

                    if (stunnedTime > 0)
                    {
                        manager.AddBuff(stunnedTime, 3);
                    }

                    if (resetSpecialCooldownOnHit)
                    {
                        manager.GM.battleManager.enemyMonsterController.specialReady[manager.GM.battleManager.enemyMonsterController.currentSlot] = true;
                        manager.GM.battleManager.enemyMonsterController.specialC[manager.GM.battleManager.enemyMonsterController.currentSlot] = 0f;
                    }
                }

                if (criticalProj)
                {
                    friend.TakeDamage(damage * 2, false);
                }
                else
                {
                    friend.TakeDamage(damage, false);
                }



                Instantiate(impactEffect, transform.position, transform.rotation);
                Destroy(gameObject);
            }
        }

        
        
        if (collision.tag == "Border")
        {
            Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}

