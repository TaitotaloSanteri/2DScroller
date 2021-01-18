using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKnight : BaseEnemy
{
    protected override void Aggressive()
    {
        if (distanceToPlayer <= attackRange)
        {
            movement = new Vector2(0f, rb.velocity.y);
            if (canAttack)
            {
                ChangeState(State.Attack);
            }
            return;
        }

        float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
        transform.localScale = new Vector3(     direction * Mathf.Abs(transform.localScale.x),
                                                transform.localScale.y,
                                                transform.localScale.z);

        movement = new Vector2(transform.localScale.x * moveSpeed, rb.velocity.y);
        if (distanceToPlayer >= aggressiveRange)
        {
            ChangeState(State.Idle);
        }
    }

    protected override void Attack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 0.95f)
        {
            canAttack = false;
            hasHitPlayer = false;
            StartCoroutine(AttackTimer());
            ChangeState(State.Aggressive);
        }
    }

    protected override void Die()
    {
        movement = new Vector2(0f, rb.velocity.y);
    }

    protected override void Idle()
    {
        movement = new Vector2(0f, rb.velocity.y);
        currentIdleTime += Time.deltaTime;
        if (currentIdleTime > maxIdleTime)
        {
            currentIdleTime = 0f;
            transform.localScale = new Vector3(-transform.localScale.x,
                                                transform.localScale.y,
                                                transform.localScale.z);
            ChangeState(State.Patrol);
        }
    }

    protected override void Patrol()
    {
        movement = new Vector2(transform.localScale.x * moveSpeed, rb.velocity.y);
        currentPatrolTime += Time.deltaTime;
        if (currentPatrolTime > maxPatrolTime)
        {
            currentPatrolTime = 0f;
            ChangeState(State.Idle);
        }
    }
}

