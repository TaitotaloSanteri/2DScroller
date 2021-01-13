using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class BaseEnemy : MonoBehaviour
{
    [SerializeField]
    protected float health, moveSpeed, attackPower;
    private State enemyState = State.Idle;
    private Rigidbody2D rb;
    private Animator animator;
    private Action currentStateMethod;

    protected enum State {
        Idle,       // Vihollinen hengaa paikoillaan
        Patrol,     // Vihollinen liikkuu, mutta ei ole huomannut pelaajaa
        Aggressive  // Vihollinen on huomannut pelaajan
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentStateMethod = Idle;
    }

    private void Update()
    {
        currentStateMethod();
    }

    protected virtual void Idle()
    {

    }

    protected virtual void Patrol()
    {

    }

    protected virtual void Aggressive()
    {

    }

    protected virtual void ChangeState(State newState)
    {
        enemyState = newState;
        switch (newState)
        {
            case State.Aggressive:
                currentStateMethod = Aggressive;
                break;
            case State.Patrol:
                currentStateMethod = Patrol;
                break;
            default:
                currentStateMethod = Idle;
                break;
        }
    }
    
    public void TakeDamage(float dmg)
    {

    }


}
