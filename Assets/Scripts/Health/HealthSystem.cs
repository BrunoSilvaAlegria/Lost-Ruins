using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    class LocalInvulnerability
    {
        public GameObject entity;
        public float      timer;
    }
    [SerializeField] private Faction _faction;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invulnerabilityDuration;

    public delegate void OnDamage(int damage, Transform damageSource);
    public event OnDamage onDamage;

    public delegate void OnInvulnerabilityToggle(bool active);
    public event OnInvulnerabilityToggle onInvulnerabilityToggle;

    public delegate void OnDeath();
    public event OnDeath onDeath;

    int                         _health = 5;
    float                       invulnerabilityTimer;
    List<LocalInvulnerability>  localInvulnerabilities = new();

    public Faction faction => _faction;
    public int     health => _health;
    public bool    isDead => _health <= 0;  

    // Start is called before the first frame update
    void Awake()
    {
        _health = maxHealth;
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

        localInvulnerabilities.ForEach((item) => item.timer -= Time.deltaTime);
        localInvulnerabilities.RemoveAll((item) => item.timer <= 0);
    }

    public bool DealDamage(int damage, Transform damageSource)
    {
        if (_health <= 0) return false;
        if (invulnerabilityTimer > 0) return false;
        if (IsInvulnerable(damageSource.gameObject)) return false;

        _health -= damage;

        onDamage?.Invoke(damage, damageSource);

        if (_health <= 0)
        {
            if (onDeath != null)
            {
                onDeath();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            invulnerabilityTimer = invulnerabilityDuration;
            if (onInvulnerabilityToggle != null) onInvulnerabilityToggle(true);
        }

        return true;
    }

    bool IsInvulnerable(GameObject entity)
    {
        foreach (var item in localInvulnerabilities)
        {
            if (item.entity == entity)
            {
                return true;
            }
        }
        return false;
    }

    public void AddInvulnerability(float duration, GameObject entity)
    {
        foreach (var item in localInvulnerabilities)
        {
            if (item.entity == entity)
            {
                item.timer = Mathf.Max(item.timer, duration);
                return;
            }
        }

        localInvulnerabilities.Add(new LocalInvulnerability() { entity = entity, timer = duration });
    }
}
