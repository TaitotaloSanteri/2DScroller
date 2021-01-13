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
            return;
        }

        float direction = Mathf.Sign(player.transform.position.x - transform.position.x);
        transform.localScale = new Vector3(     direction * Mathf.Abs(transform.localScale.x),
                                                transform.localScale.y,
                                                transform.localScale.z);
        movement = new Vector2(transform.localScale.x * moveSpeed, rb.velocity.y);
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

