using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    //AUDIO
    [Header("Audio")]
    [SerializeField] private AudioSource hurtSnd;

    //INVULNERABILITY 
    [Header("Invulnerability")]
    [SerializeField] private float blinkDuration = 0.1f;
    private float blinkTimer;
    private bool isGrounded = false;

    //KNOCKBACK
    [Header("Knockback")]
    [SerializeField] private float velocityPerDamage = 100.0f;
    [SerializeField] private float maxKnockbackVelocity = 200.0f;
    [SerializeField] private float timeScaleDuration = 0.1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private HealthSystem health;
    private Animator anim;
    
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        health = GetComponent<HealthSystem>();
        anim = GetComponent<Animator>();

        if (health != null)
        {
            health.onDamage += EnemyTookDamage;
            health.onInvulnerabilityToggle += ToggleInvulnerability;
            health.onDeath += EnemyDied;
        }
    }

    private void OnDestroy()
    {
        if (health)
        {
            health.onInvulnerabilityToggle -= ToggleInvulnerability;
            health.onDeath -= EnemyDied;
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

    void EnemyDied()
    {
        StartCoroutine(EnemyDiedCR());
    }

    IEnumerator EnemyDiedCR()
    {
        anim.SetTrigger("DEAD"); //Death animation

        yield return new WaitForSeconds(1.5f);
        
        /*float timer = 0f;
        while (timer < 1.5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }*/
        Destroy(gameObject);
        this.enabled = false;
    }

    void EnemyTookDamage(int damage, Transform damageSource)
    {
        if (health.isDead) return;

        StartCoroutine(EnemyTookDamageCR(damage, damageSource));
    }

    IEnumerator EnemyTookDamageCR(int damage, Transform damageSource)
    {
        // Passo 0: Tocar o som
        if (hurtSnd != null) hurtSnd.Play();
        //Passo 0.5: Correr a animação
        anim.SetTrigger("HURT");
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
}
