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
    maxPatrolTime, visionRange = 5f, attackRange = 1f, aggressiveRange = 35f;
    protected float currentIdleTime = 0f, currentPatrolTime = 0f, distanceToPlayer;
    protected bool hasLineOfSight, isDead = false;
    protected Transform player;
    protected Vector2 movement;
    protected Rigidbody2D rb;
    protected Animator animator;

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
        if (isDead) return;
        CheckLineOfSight();
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
            default:
                currentStateMethod = Idle;
                animator.SetBool("Walk", false);
                break;
        }
    }
    
    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0f)
        {
            animator.SetTrigger("Die");
            isDead = true;
        }
    }


}
