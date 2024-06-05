using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private Faction _faction;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invulnerabilityDuration;

    public Faction faction => _faction;

    public delegate void OnInvulnerabilityToggle(bool active);
    public event OnInvulnerabilityToggle onInvulnerabilityToggle;

    public delegate void OnDeath();
    public event OnDeath onDeath;

    int health;
    float invulnerabilityTimer;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0.0f)
            {
                if (onInvulnerabilityToggle != null) onInvulnerabilityToggle(false);
            }
        }
    }

    public bool DealDamage(int damage)
    {
        if (health <= 0) return false;
        if (invulnerabilityTimer > 0) return false;

        health -= damage;

        if (health <= 0)
        {
            onDeath?.Invoke();
        }
        else
        {
            invulnerabilityTimer = invulnerabilityDuration;
            if (onInvulnerabilityToggle != null) onInvulnerabilityToggle(true);
        }

        return true;
    }
}
