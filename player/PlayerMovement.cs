using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;

    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Wall Jump Settings")]
    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpForce = new Vector2(10f, 15f);
    public Transform wallCheckLeft;
    public Transform wallCheckRight;
    public float wallCheckDistance = 0.5f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool isDashing;
    private bool isWallSliding;
    private bool isTouchingWallLeft;
    private bool isTouchingWallRight;
    private bool isTouchingWall;
    private float dashCooldownTimer;
    private float moveInput;
    private bool facingRight = true;

    private PlayerHealth playerHealth;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth.GetKnockbackState()) return;  // 🚫 Stop movement during knockback
        if (!isDashing)
        {
            HandleMovement();
            HandleJumping();
            HandleWallSlideAndJump();
            HandleDash();
        }
    }

    void FixedUpdate()
    {
        CheckSurroundings();
    }

    void HandleMovement()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        if (facingRight && moveInput < 0) Flip();
        else if (!facingRight && moveInput > 0) Flip();
    }

    void HandleJumping()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = true;
            }
            else if (canDoubleJump && !isWallSliding)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = false;
            }
        }
    }

    void HandleWallSlideAndJump()
    {
        if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
        }
        else
        {
            isWallSliding = false;
        }

        if (Input.GetButtonDown("Jump") && isWallSliding)
        {
            rb.velocity = new Vector2(-GetWallDirection() * wallJumpForce.x, wallJumpForce.y);
            FlipWallJump(); // Flip player after wall jump
        }
    }

    void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0)
        {
            StartCoroutine(Dash());
        }
        else
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2((facingRight ? 1 : -1) * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
        dashCooldownTimer = dashCooldown;
    }

    void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchingWallLeft = Physics2D.Raycast(wallCheckLeft.position, Vector2.left, wallCheckDistance, wallLayer);
        isTouchingWallRight = Physics2D.Raycast(wallCheckRight.position, Vector2.right, wallCheckDistance, wallLayer);
        isTouchingWall = isTouchingWallLeft || isTouchingWallRight;
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    void FlipWallJump()
    {
        // Flip only if player is jumping from the opposite direction
        if ((facingRight && isTouchingWallLeft) || (!facingRight && isTouchingWallRight))
        {
            Flip();
        }
    }

    int GetWallDirection()
    {
        return isTouchingWallRight ? 1 : -1;
    }
}
