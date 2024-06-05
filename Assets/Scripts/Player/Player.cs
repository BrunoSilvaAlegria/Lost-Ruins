using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Player : MonoBehaviour
{
    enum WallType { Unknown, Left, Right };
    //MOVEMENT
    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpSpeed = 10f;

    [Header("Ground")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeReference] private float groundRadius = 0.2f;
    private bool isGrounded = false;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpTime = 0.2f;
    [SerializeField] private float wallSlideSpeed = 0.3f;
    [SerializeField] private float wallDistance = 0.55f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    private bool isWallSliding = false;
    private bool canWallJump = true;
    private string previousJumpedWall = "";
    RaycastHit2D wallCheckHit;

    [SerializeField] private float jumpTime;

    private float hzInput;

    private bool isFacingRight = true;

    //COMBAT
    [Header("Melee Attack")]
    [SerializeField] public int meleeDamage = 2;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 2;
    [SerializeField] LayerMask enemyLayer;

    [Header("Arrows Shooting")]
    [SerializeField] private Transform bowPos;
    [SerializeField] private GameObject arrowPrefab;


    //Invulnerability
    [Header("Invulnerability")]
    [SerializeField] private float blinkDuration = 0.1f;
    private float blinkTimer;

    private HealthSystem health;

    private List<int> arrowAmount = new List<int>(5);
    private bool arrowShot = false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private WallType wallType = WallType.Unknown;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = GetComponent<HealthSystem>();

        health.onInvulnerabilityToggle += ToggleInvulnerability;
        health.onDeath += PlayerDied;
    }

    private void OnDestroy()
    {
        if (health)
        {
            health.onInvulnerabilityToggle -= ToggleInvulnerability;
            health.onDeath -= PlayerDied;
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

    private void Update()
    {
        if (isGrounded && Input.GetButtonDown("Jump") || isWallSliding && Input.GetButtonDown("Jump")) Jump();
        if (blinkTimer > 0)
        {
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0)
            {
                sr.enabled = !sr.enabled;
                blinkTimer = blinkDuration;
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse0)) BowShot();

    }

    private void FixedUpdate()
    {
        //MOVEMENT

        hzInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(hzInput * moveSpeed, rb.velocity.y);

        if (hzInput < 0f)
        {
            isFacingRight = false;
            transform.localScale = new Vector2 (-1f, transform.localScale.y);
        }

        else if (hzInput > 0f)
        {
            isFacingRight = true;
            transform.localScale = new Vector2(1f, transform.localScale.y);
        }
        Flip();

        bool touchingGround = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        if (touchingGround)
        {
            isGrounded = true;
            jumpTime = Time.time + wallJumpTime;
            wallType = WallType.Unknown;
        }
        else if (!touchingGround && jumpTime < Time.time) isGrounded = false;

        //WALL JUMP

        if (isFacingRight)
        {
            wallCheckHit = Physics2D.Raycast(transform.position, new Vector2(wallDistance, 0f), wallDistance, groundLayer);
            //Debug.DrawRay(transform.position, new Vector2(wallDistance, 0f), Color.red);
        }
        else if (!isFacingRight)
        {
            wallCheckHit = Physics2D.Raycast(transform.position, new Vector2(-wallDistance, 0f), wallDistance, groundLayer);
            //Debug.DrawRay(transform.position, new Vector2(-wallDistance, 0f), Color.red);
        }

        if (wallCheckHit && !isGrounded && hzInput != 0f)
        {
            isWallSliding = true;
            jumpTime = Time.time + wallJumpTime;
        }
        else if (jumpTime < Time.time) isWallSliding = false;


        if (isWallSliding == true)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallSlideSpeed, float.MaxValue));

        }

        //Debug.Log($"wallType={wallType}, canWallJump={canWallJump}");

        //SHOOTING

        if (arrowShot)
        {
            foreach (int arrow in arrowAmount)
            {
                arrowAmount.RemoveAt(arrow);
            }
        }


        //MELEE

        if (Input.GetKeyDown(KeyCode.E)) MeleeAttack();

        //ANIMATION

        anim.SetBool("RUN", hzInput != 0);
        anim.SetBool("GROUNDED", groundCheck);
        anim.SetBool("WALL SLIDE", isWallSliding == true);
    }

    private void BowShot()
    {
        Instantiate(arrowPrefab, bowPos.position, bowPos.rotation);
    }
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

        if (isGrounded) anim.SetTrigger("JUMP");
        else if (isWallSliding == true && canWallJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            canWallJump = false;
            anim.SetTrigger("WALL JUMP");
        }

        
    }
    private void Flip()
    {
        if (isFacingRight && hzInput < 0f || !isFacingRight && hzInput > 0f)
        {
            isFacingRight = !isFacingRight;

            transform.Rotate(0f, 180f, 0f);
        }
    }

    /// <summary>
    /// Doesn't let the player use the wall jump on the same wall over and over again
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if ((wallType == WallType.Unknown) ||
                ((wallType == WallType.Right) && (!isFacingRight)) ||
                ((wallType == WallType.Left) && (isFacingRight)))
            {
                previousJumpedWall = other.gameObject.name;
                canWallJump = true;
                if (isFacingRight)
                {
                    wallType = WallType.Right;
                }
                else
                {
                    wallType = WallType.Left;
                }
            }
        }
    }

    //MELEE ATTACK

    void MeleeAttack()
    {
        //Animation
        anim.SetTrigger("MELEE ATTACK");

        //Enemy detection
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        //Damage enemy
        foreach (Collider2D enemy in enemiesHit)
        {
            //Debug.Log($"{enemy.name} hit");
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}