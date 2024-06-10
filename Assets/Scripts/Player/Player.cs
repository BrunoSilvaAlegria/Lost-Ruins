using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Player : MonoBehaviour
{
    enum WallType { Unknown, Left, Right };

    //AUDIO
    [Header("Audio")]
    [SerializeField] private AudioSource walkSound;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource hurtSnd;

    //MOVEMENT
    [Header("Horizontal Movement")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("Vertical Movement")]
    [SerializeField] private float jumpSpeed = 10f;

    [Header("Ground")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeReference] private float groundRadius = 0.2f;
    [SerializeField] private Collider2D groundCollider;
    [SerializeField] private Collider2D airCollider;
    private bool isGrounded = false;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpTime = 0.2f;
    [SerializeField] private float wallSlideSpeed = 0.3f;
    [SerializeField] private float wallDistance = 0.55f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float jumpTime;

    private bool isWallSliding = false;
    private bool canWallJump = true;
    private string previousJumpedWall = "";
    RaycastHit2D wallCheckHit;

    private float hzInput;
    private bool isFacingRight = true;
    private bool collisionEnable = true;

    private bool animStopper = true;

    //COMBAT
    [Header("Melee Attack")]
    [SerializeField] private int meleeDamage = 2;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] LayerMask enemyLayer;

    [Header("Arrows Shooting")]
    [SerializeField] private Transform bowPos;
    [SerializeField] private GameObject arrowPrefab;


    //INVULNERABILITY 
    [Header("Invulnerability")]
    [SerializeField] private float blinkDuration = 0.1f;
    private float blinkTimer;

    //KNOCKBACK
    [Header("Knockback")]
    [SerializeField] private float velocityPerDamage = 100.0f;
    [SerializeField] private float maxKnockbackVelocity = 200.0f;
    [SerializeField] private float timeScaleDuration = 0.1f;


    //private List<int> arrowAmount = new List<int>(5);
    //private bool arrowShot = false;
    private Scene currentScene;
    private HealthSystem health;
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

    }

    private void Start()
    {
        health.onDamage += PlayerTookDamage;
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
        StartCoroutine(PlayerDiedCR());
    }

    IEnumerator PlayerDiedCR()
    {
        collisionEnable = false;
        anim.SetTrigger("LARA DEAD");

        float timer = 0f;
        while(timer < 1.5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);

    }

    void PlayerTookDamage(int damage, Transform damageSource)
    {
        if (health.isDead) return;

        StartCoroutine(PlayerTookDamageCR(damage, damageSource));
    }

    IEnumerator PlayerTookDamageCR(int damage, Transform damageSource)
    {
        // Passo 0: Tocar o som
        if (hurtSnd != null) hurtSnd.Play();
        //Passo 0.5: Correr a animação
        anim.SetTrigger("LARA HURT");
        // Passo 1: Mudar a velocidade para o knockback
        float velocityX = Mathf.Clamp(damage * velocityPerDamage, 0.0f, maxKnockbackVelocity);
        float velocityY = velocityX * 2.0f;
        if (damageSource.position.x > transform.position.x) velocityX = -velocityX;
        rb.velocity = new Vector2(velocityX, velocityY);
        // Passo 2: Stutter
        if (timeScaleDuration > 0)
        {
            Time.timeScale = 0.001f;
            yield return new WaitForSecondsRealtime(timeScaleDuration);
            Time.timeScale = 1.0f;
        }
        // Passo 3: Esperar X tempo
        //yield return new WaitForSeconds(knockbackTime);

        yield return new WaitForSeconds(0.1f);
        while (isGrounded)
        {
            yield return null;
        }
    }

    private void Update()
    {   
        if (blinkTimer > 0)
        {
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0)
            {
                sr.enabled = !sr.enabled;
                blinkTimer = blinkDuration;
            }
        }

        airCollider.enabled = !isGrounded && collisionEnable;
        groundCollider.enabled = isGrounded && collisionEnable;

        if (isGrounded && Input.GetButtonDown("Jump") || isWallSliding && Input.GetButtonDown("Jump")) Jump();
        if (Input.GetKeyDown(KeyCode.K)) BowShot();
        if (Input.GetKeyDown(KeyCode.J)) MeleeAttack();

    }

    private void FixedUpdate()
    {
        //MOVEMENT

        hzInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(hzInput * moveSpeed, rb.velocity.y);

        if (hzInput < 0f)
        {
            walkSound.Play();
            isFacingRight = false;
            transform.localScale = new Vector2 (-1f, transform.localScale.y);
        }

        else if (hzInput > 0f)
        {
            walkSound.Play();
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

        /*if (arrowShot)
        {
            foreach (int arrow in arrowAmount)
            {
                arrowAmount.RemoveAt(arrow);
            }
        }*/

        //ANIMATION

        anim.SetBool("RUN", hzInput != 0);
        anim.SetBool("GROUNDED", groundCheck);
        anim.SetBool("WALL SLIDE", isWallSliding == true);
    }

    private void Jump()
    {
        jumpSound.Play();
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
        animStopper = true;

        //Animation
        if (animStopper)
        {
            anim.SetTrigger("MELEE ATTACK");
            animStopper = false;
        }

        //Enemy detection
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        //Damage enemy
        foreach (Collider2D enemy in enemiesHit)
        {
            //Debug.Log($"{enemy.name} hit");
            enemy.GetComponent<HealthSystem>().DealDamage(meleeDamage, gameObject.transform);
        }
    }

    private void BowShot()
    {
        animStopper = true;

        //ANIMATION
        if (animStopper)
        {
            anim.SetTrigger("BOW SHOT");
            animStopper = false;
        }

        GameObject arrowObject = Instantiate(arrowPrefab, bowPos.position, bowPos.rotation);
        Arrow arrow = arrowObject.GetComponent<Arrow>();
        arrow.SetDirection(isFacingRight ? Vector2.right : Vector2.left);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}