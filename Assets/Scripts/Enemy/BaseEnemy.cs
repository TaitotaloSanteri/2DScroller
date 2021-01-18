using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseEnemy : MonoBehaviour
{
    [SerializeField]
    protected float health, moveSpeed, attackPower, maxIdleTime, 
    maxPatrolTime, visionRange = 5f, attackRange = 1f, aggressiveRange = 35f, invulTime = 0.5f,
    recoilX, recoilY, attackDelay = 0.3f, attackMultiplier = 2f;
    protected float currentIdleTime = 0f, currentPatrolTime = 0f, distanceToPlayer;
    protected bool hasLineOfSight, invul, hasHitPlayer, canAttack = true;
    protected Transform player;
    protected Vector2 movement;
    protected Rigidbody2D rb;
    protected Animator animator;

    private Collider2D[] colliders;
    private State enemyState = State.Idle;
    private Action currentStateMethod;

    protected enum State {
        Idle,       // Vihollinen hengaa paikoillaan
        Patrol,     // Vihollinen liikkuu, mutta ei ole huomannut pelaajaa
        Aggressive,  // Vihollinen on huomannut pelaajan
        Attack,
        Die
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentStateMethod = Idle;
        colliders = GetComponentsInChildren<Collider2D>();
        animator.SetFloat("AttackMultiplier", attackMultiplier);
    }
    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    private void FixedUpdate()
    {
        rb.velocity = movement;
    }

    private void Update()
    {
        if (enemyState != State.Die && enemyState != State.Attack)
        {
            CheckLineOfSight();
        }
        currentStateMethod();
    }

    private void CheckLineOfSight()
    {
        distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        RaycastHit2D raycast = Physics2D.Linecast(transform.position, player.transform.position);

        if (raycast)
        {
            if (distanceToPlayer < visionRange && enemyState != State.Aggressive)
            {
                ChangeState(State.Aggressive);
            }
        }
    }


    protected abstract void Idle();
    protected abstract void Patrol();
    protected abstract void Aggressive();
    protected abstract void Attack();
    protected abstract void Die();


    protected virtual void ChangeState(State newState)
    {
        enemyState = newState;
        switch (newState)
        {
            case State.Aggressive:
                currentStateMethod = Aggressive;
                animator.SetBool("Walk", true);
                break;

            case State.Patrol:
                currentStateMethod = Patrol;
                animator.SetBool("Walk", true);
                break;

            case State.Attack:
                currentStateMethod = Attack;
                animator.SetTrigger("Attack");
                break;

            case State.Die:
                currentStateMethod = Die;
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Die");
                rb.isKinematic = true;
                foreach (Collider2D col in colliders)
                {
                    col.enabled = false;
                }
                break;

            default:
                currentStateMethod = Idle;
                animator.SetBool("Walk", false);
                break;
        }
    }
    
    public void TakeDamage(float dmg)
    {
        if (invul) return;

        UIManager.instance.ShowDamageText(dmg.ToString(), transform.position, Color.green);
        health -= dmg;
        invul = true;
        StartCoroutine(InvulTimer());
        //Vector2 force = -transform.localScale.x * Vector2.right * recoilX + Vector2.up * recoilY;
        //rb.AddForce(force);
        animator.ResetTrigger("Attack");
        animator.SetTrigger("GetHit");
        if (health <= 0f)
        {
            ChangeState(State.Die);
        }
    }

    private IEnumerator InvulTimer()
    {
        yield return new WaitForSeconds(invulTime);
        invul = false;
    }

    protected IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (!hasHitPlayer && enemyState == State.Attack && collision.gameObject.layer == 10)
        {
            hasHitPlayer = true;
            player.GetComponent<PlayerController>().TakeDamage(attackPower);
        }
    }

}
