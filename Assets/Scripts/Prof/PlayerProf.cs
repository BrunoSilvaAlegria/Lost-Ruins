using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    private float blinkDuration = 0.1f;
    [SerializeField]
    private float velocityPerDamage = 100.0f;
    [SerializeField]
    private float maxKnockbackVelocity = 200.0f;
    [SerializeField]
    private float timeScaleDuration = 0.1f;
    [SerializeField]
    private AudioSource hurtSnd;
    [SerializeField]
    private float interactionRadius = 40.0f;
    [SerializeField]
    private LayerMask interactionMask;
    [SerializeField]
    private TextMeshProUGUI interactionText;

    private Rigidbody2D     rb;
    private SpriteRenderer  sr;
    private Animator        animator; 
    private float           defaultGravity;
    private float           jumpTime;
    private HealthSystem    hs;
    private float           blinkTimer;
    private bool            inputEnable = true;
    private bool            collisionEnable = true;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        hs = GetComponent<HealthSystem>();

        defaultGravity = rb.gravityScale;
    }

    private void Start()
    {
        hs.onDamage += PlayerTookDamage;
        hs.onInvulnerabilityToggle += ToggleInvulnerability;
        hs.onDeath += PlayerDied;

        TouchDamage touchDamage = GetComponent<TouchDamage>();
        if (touchDamage != null)
        {
            touchDamage.onDamage += OnCrushEnemy;
        }
    }

    void OnCrushEnemy(int damage, HealthSystem target)
    {
        Vector2 velocity = rb.velocity;
        velocity.y = jumpSpeed * 0.5f;
        rb.velocity = velocity;
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
        StartCoroutine(PlayerDiedCR());
    }

    IEnumerator PlayerDiedCR()
    {
        collisionEnable = false;
        inputEnable = false;
        rb.velocity = new Vector2(0, jumpSpeed * 0.5f);
        animator.SetTrigger("isDead");

        ObjectFollow objectFollow = FindObjectOfType<ObjectFollow>();
        if (objectFollow.objectToFollow == transform)
        {
            objectFollow.objectToFollow = null;
        }

        float timer = 0.0f;
        while (timer < 1.5f)
        {
            timer += Time.deltaTime;

            float t = timer / 1.5f;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 5, Mathf.Sqrt(t));
            yield return null;
        }

        Destroy(gameObject);
    }

    void PlayerTookDamage(int damage, Transform damageSource)
    {
        if (hs.isDead) return;

        StartCoroutine(PlayerTookDamageCR(damage, damageSource));
    }

    IEnumerator PlayerTookDamageCR(int damage, Transform damageSource)
    {
        // Passo 0: Tocar o som
        if (hurtSnd != null) hurtSnd.Play();
        // Passo 1: Mudar a velocidade para o knockback
        float velocityX = Mathf.Clamp(damage * velocityPerDamage, 0.0f, maxKnockbackVelocity);
        float velocityY = velocityX * 2.0f;
        if (damageSource.position.x > transform.position.x) velocityX = -velocityX;
        rb.velocity = new Vector2(velocityX, velocityY);
        // Passo 2: Desactivar o input
        inputEnable = false;
        // Passo 2.5: Stutter
        if (timeScaleDuration > 0)
        {
            Time.timeScale = 0.001f;
            yield return new WaitForSecondsRealtime(timeScaleDuration);
            Time.timeScale = 1.0f;
        }
        // Passo 3: Esperar X tempo
        //yield return new WaitForSeconds(knockbackTime);

        yield return new WaitForSeconds(0.1f);
        while (!IsGrounded())
        {
            yield return null;
        }
        // Passo 4: Activar o input
        inputEnable = true;
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

        airCollider.enabled = !isGrounded && collisionEnable;
        groundCollider.enabled = isGrounded && collisionEnable;

        Vector3 currentVelocity = rb.velocity;

        if (inputEnable)
        {
            float deltaX = Input.GetAxis("Horizontal");
            //        Vector3 moveVector = new Vector3(deltaX * moveSpeed * Time.deltaTime, 0, 0);
            //        transform.position = transform.position + moveVector;

            currentVelocity.x = deltaX * moveSpeed;

            if ((Input.GetButtonDown("Jump")) && (isGrounded))
            {
                currentVelocity.y = jumpSpeed;
                rb.gravityScale = 1.0f;
                jumpTime = Time.time;
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

            // Animation
            animator.SetFloat("AbsVelocityX", Mathf.Abs(currentVelocity.x));

            if ((currentVelocity.x < 0) && (transform.right.x > 0)) transform.rotation = Quaternion.Euler(0, 180, 0);
            else if ((currentVelocity.x > 0) && (transform.right.x < 0)) transform.rotation = Quaternion.identity;
        }

        animator.SetFloat("VelocityY", currentVelocity.y);
        animator.SetBool("isGrounded", isGrounded);

        //UseMechanic();
    }

    /*void UseMechanic()
    { 
        bool         canInteract = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactionMask);
        foreach (var collider in colliders)
        {
            Interactable interactable = collider.GetComponent<Interactable>();
            if (interactable)
            {
                if (interactable.CanInteract(this))
                {
                    if (Input.GetButtonDown("Use"))
                    {
                        interactable.Interact(this);
                    }

                    interactionText.text = interactable.displayText;
                    canInteract = true;
                }
            }
        }

        if (!canInteract)
        {
            interactionText.text = "";
        }
        interactionText.transform.rotation = Quaternion.identity;
    }*/

    private bool IsGrounded()
    {
        Collider2D collider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundCheckLayers);

        return (collider != null);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
