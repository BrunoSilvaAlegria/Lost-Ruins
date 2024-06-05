using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerProf : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 100;
    [SerializeField]
    private float jumpSpeed = 200;
    [SerializeField]
    private float maxJumpTime = 0.1f;
    [SerializeField]
    private Collider2D groundCollider;
    [SerializeField]
    private Collider2D airCollider;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundCheckRadius = 2;
    [SerializeField]
    private LayerMask groundCheckLayers;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private float wallCheckRadius = 2;
    [SerializeField]
    private LayerMask wallCheckLayers;
    [SerializeField]
    private float blinkDuration = 0.1f;
    [SerializeField]
    private float wallJumpingTime = 0.2f;

    private Rigidbody2D     rb;
    private SpriteRenderer  sr;
    private Animator        animator;
    private float           defaultGravity;
    private float           jumpTime;
    private HealthSystem    hs;
    private float           blinkTimer;
    private float           deltaX;


    private bool            isWallJumping;
    private float           wallJumpingDirection;
    private float           wallJumpingCounter;
    private float           wallJumpingDuration = 0.4f;
    private Vector2         wallJumpPower = new Vector2(8f, 16f);

    private bool            isWallSliding;
    private float           wallSlidingSpeed;

    private bool            isFacingRight = true;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        hs = GetComponent<HealthSystem>();

        hs.onInvulnerabilityToggle += ToggleInvulnerability;
        hs.onDeath += PlayerDied;

        defaultGravity = rb.gravityScale;
    }


    private void OnDestroy()
    {
        if (hs)
        {
            hs.onInvulnerabilityToggle -= ToggleInvulnerability;
            hs.onDeath -= PlayerDied;
        }
    }

    void ToggleInvulnerability(bool active)
    {
        if (active)
        {
            blinkTimer = blinkDuration;
        }
        else
        {
            blinkTimer = 0;
            sr.enabled = true;
        }
    }

    void PlayerDied()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        bool isGrounded = IsGrounded();

        if (blinkTimer > 0)
        {
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0)
            {
                sr.enabled = !sr.enabled;
                blinkTimer = blinkDuration;
            }
        }

        airCollider.enabled = !isGrounded;
        groundCollider.enabled = isGrounded;

        deltaX = Input.GetAxis("Horizontal");
        //        Vector3 moveVector = new Vector3(deltaX * moveSpeed * Time.deltaTime, 0, 0);
        //        transform.position = transform.position + moveVector;

        Vector3 currentVelocity = rb.velocity;

        currentVelocity.x = deltaX * moveSpeed;

        if ((Input.GetButtonDown("Jump")) && (isGrounded))
        {
            currentVelocity.y = jumpSpeed;
            rb.gravityScale = 1.0f;
            jumpTime = Time.time;

            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        else if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2 (rb.velocity.x, rb.velocity.y * 0.5f);
        }
        else if ((Input.GetButton("Jump")) && ((Time.time - jumpTime) < maxJumpTime))
        {
            rb.gravityScale = 1.0f;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }

        rb.velocity = currentVelocity;

        WallSlide();
        WallJump();
        
        if (!isWallJumping)
        {
            Flip();
        }

        // Animation
        animator.SetFloat("AbsoluteVelX", Mathf.Abs(currentVelocity.x));

        if ((currentVelocity.x < 0) && (transform.right.x > 0)) transform.rotation = Quaternion.Euler(0, 180, 0);
        else if ((currentVelocity.x > 0) && (transform.right.x < 0)) transform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(deltaX * jumpSpeed, rb.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundCheckLayers);

        return (collider != null);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallCheckLayers);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && deltaX != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && deltaX < 0f || !isFacingRight && deltaX > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        }
        else if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(wallCheck.position, wallCheckRadius);
        }
    }
}