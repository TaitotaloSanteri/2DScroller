using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f, jumpForce = 50f, maxHealth = 100f, damage = 5f;
    private float xMovement, xScale, currentHealth;

    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField]
    private SpriteRenderer rightAxe, leftAxe;

    private bool isAttacking;

    // Hyppyyn liittyvät muuttujat
    private bool isJumpButtonPressed, isGrounded, canJump, isJumping;
    private float currentAirTime = 0f;
    [SerializeField]
    private float maxJumpTime = 0.25f;


    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        xScale = transform.localScale.x;
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(xMovement * moveSpeed, rb.velocity.y);
        if (isJumpButtonPressed && isJumping)
        {
            currentAirTime += Time.fixedDeltaTime;
            rb.AddForce(Vector2.up * jumpForce);

            if (currentAirTime >= maxJumpTime)
            {
                isJumping = false;
                canJump = false;
                currentAirTime = 0f;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.OnPause();
        }

        HandleMovement();
        HandleJump();
        HandleAttack();
    }

    private void HandleAttack()
    {
        if (Input.GetAxisRaw("Fire1") > 0f && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
        }
        float x = -Mathf.Sign(transform.localScale.x);
        animator.SetFloat("AttackArm", x + 1);
    }


    private void HandleJump()
    {
        isJumpButtonPressed = Input.GetAxisRaw("Jump") > 0f;
        if (isJumpButtonPressed && canJump)
        {
            animator.SetBool("Jump", true);
            isJumping = true;
        }

        else if (!isJumpButtonPressed && !isGrounded && isJumping)
        {
            canJump = false;
            isJumping = false;
            currentAirTime = 0f;
        }
    }

    public void StopAttacking()
    {
        isAttacking = false;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UIManager.instance.UpdateHealthBar(currentHealth, maxHealth);
        UIManager.instance.ShowDamageText(damage.ToString(), transform.position, Color.red);
    }

    private void HandleMovement()
    {
        xMovement = Input.GetAxisRaw("Horizontal");
        if (xMovement != 0f)
        {
            transform.localScale = new Vector3(xScale * xMovement,
                                               transform.localScale.y,
                                               transform.localScale.z);
        }
        animator.SetBool("Walk", xMovement != 0f);
        animator.SetFloat("WalkMultiplier", moveSpeed * 0.2f);

        rightAxe.enabled = transform.localScale.x > 0f;
        leftAxe.enabled = !rightAxe.enabled;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            canJump = true;
            animator.SetBool("Jump", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacking)
        {
            BaseEnemy enemy = collision.GetComponentInParent<BaseEnemy>();
            enemy?.TakeDamage(damage);
            
        }
    }


}


