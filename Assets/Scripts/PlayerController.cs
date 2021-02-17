using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f, jumpForce = 50f, maxHealth = 100f, damage = 5f;
    private float xMovement, xScale, currentHealth;
    [SerializeField]
    private Transform ledgeCheck, spaceCheck;
    private Rigidbody2D rb;
    private Animator animator;
    private static Vector3 currentCheckpoint = Vector3.zero;
    [SerializeField]
    private SpriteRenderer rightAxe, leftAxe;
    private int groundLayerMask;
    private bool isAttacking, isHanging, isClimbing;
    private Vector2 climbTarget;
    // Hyppyyn liittyvät muuttujat
    private bool isJumpButtonPressed, isGrounded, canJump, isJumping;
    private float currentAirTime = 0f;
    [SerializeField]
    private float maxJumpTime = 0.25f;


    private void Awake()
    {
        transform.position = currentCheckpoint == Vector3.zero ? transform.position : currentCheckpoint;
        
        // Asetetaan groundLayerMask vastaamaan Default layeriä. Tätä tarvitaan
        // ledgeCheckiä varten.
        groundLayerMask = LayerMask.GetMask("Default");
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

        if (!isHanging)
        {
            HandleMovement();
        }
        else
        {
            HandleClimbing();
        }
        HandleJump();
        HandleAttack();
    }

    private void HandleClimbing()
    {
        if (isClimbing)
        {
            rb.position = Vector2.MoveTowards(rb.position, climbTarget, moveSpeed * 2f * Time.deltaTime);
            if (rb.position == climbTarget)
            {
                isClimbing = false;
                isHanging = false;
                rb.isKinematic = false;
                animator.ResetTrigger("Hang");
                animator.SetTrigger("StopHang");
            }
            return;
        }

        rb.velocity = Vector2.zero;
        if (Input.GetAxisRaw("Vertical") > 0f)
        {
            isClimbing = true;
        }
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
        if (isHanging)
        {
            return;
        }

        if (!isHanging)
        {
            // Tarkistetaan LedgeCheck ja SpaceChecking osumat. Käyetään groundLayerMask
            // muuttujaa siihen, että testataan osumat vain Default layerissä olevia
            // collidereja vastaan.
            Collider2D isLedgeCheck = Physics2D.OverlapCircle(ledgeCheck.position, 0.2f, groundLayerMask);
            bool isSpaceCheck = Physics2D.OverlapCircle(spaceCheck.position, 0.2f, groundLayerMask);
            bool isOnLedge = isLedgeCheck && !isSpaceCheck;
            if (isOnLedge)
            {
                isHanging = true;
                animator.SetBool("Jump", false);
                animator.SetTrigger("Hang");
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                xMovement = 0f;
                Vector2 horizontalOffset = Mathf.Sign(transform.localScale.x) * Vector2.right * 0.5f;
                Vector2 hangingPosition = isLedgeCheck.ClosestPoint(ledgeCheck.position);
                rb.position = hangingPosition + Vector2.down * 2f - horizontalOffset;
                climbTarget = hangingPosition + Vector2.up * 2f + horizontalOffset;
                return;
            }
        }

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
        
        if (currentHealth <= 0f)
        {
            SceneManager.LoadScene("SampleScene");
        }
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
        if (collision.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.transform.position;
        }

        if (isAttacking)
        {
            BaseEnemy enemy = collision.GetComponentInParent<BaseEnemy>();
            enemy?.TakeDamage(damage);
        }
    }


}


