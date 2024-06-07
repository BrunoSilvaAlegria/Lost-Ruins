using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;

public class TouchDamage : MonoBehaviour
{
    [SerializeField] private Faction faction;
    [SerializeField] private int damage = 1;
    [SerializeField] private bool destroyOnDamage = false;
    [SerializeField] private bool localInvulnerability = false;
    [SerializeField] private int damageDelayInFrames = 0;
    [SerializeField] private bool directionalAttack = false;
    [SerializeField]
    [ShowIf(nameof(directionalAttack))]
    private Vector2 normalizedDirection = Vector2.up;
    [SerializeField, Range(0.0f, 180.0f)]
    [ShowIf(nameof(directionalAttack))]
    private float angleTolerance = 45.0f;

    bool testFunc => directionalAttack && (angleTolerance > 40);

    public delegate void OnDamage(int damage, HealthSystem target);
    public event OnDamage onDamage;

    HealthSystem thisHealthSystem;
    Rigidbody2D thisRigidBody;

    private void Awake()
    {
        thisHealthSystem = GetComponent<HealthSystem>();
        thisRigidBody = GetComponent<Rigidbody2D>();

        normalizedDirection.Normalize();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        HealthSystem otherHealthSystem = collider.GetComponent<HealthSystem>();
        if (otherHealthSystem == null) otherHealthSystem = collider.GetComponentInParent<HealthSystem>();
        if (otherHealthSystem != null)
        {
            Debug.Log($"Collided with health system  - {name}/{collider.name}");
            if (faction.IsHostile(otherHealthSystem.faction))
            {
                if (directionalAttack)
                {
                    Vector2 hitDirection = (transform.position - otherHealthSystem.transform.position).normalized;
                    float dp = Vector2.Dot(normalizedDirection, hitDirection);
                    float angle = Mathf.Acos(dp) * Mathf.Rad2Deg;

                    Debug.Log($"{name} hit {otherHealthSystem.name} at a {angle} angle");
                    if (angle >= angleTolerance) return;

                    if (thisRigidBody)
                    {
                        Vector2 velocity = -thisRigidBody.velocity.normalized;
                        dp = Vector2.Dot(normalizedDirection, velocity);
                        angle = Mathf.Acos(dp) * Mathf.Rad2Deg;

                        Debug.Log($"{name} hit {otherHealthSystem.name} with velocity at a {angle} angle");
                        if (angle >= angleTolerance) return;
                    }
                }

                if (damageDelayInFrames > 0)
                {
                    StartCoroutine(ExecuteDamageCR(damage, otherHealthSystem));
                }
                else
                {
                    ExecuteDamage(damage, otherHealthSystem);
                }
            }
        }
    }

    IEnumerator ExecuteDamageCR(int damage, HealthSystem entity)
    {
        for (int i = 0; i < damageDelayInFrames; i++)
        {
            yield return null;
        }

        ExecuteDamage(damage, entity);
    }

    void ExecuteDamage(int damage, HealthSystem entity)
    { 
        if (entity.DealDamage(damage, transform))
        {
            onDamage?.Invoke(damage, entity);

            if (destroyOnDamage)
            {
                Destroy(gameObject);
            }

            if ((localInvulnerability) && (thisHealthSystem))
            {
                thisHealthSystem.AddInvulnerability(0.1f, entity.gameObject);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (directionalAttack)
        {
            Handles.color = new Color(0.0f, 1.0f, 0.5f, 0.05f);
            Handles.DrawSolidArc(transform.position, Vector3.forward, normalizedDirection, angleTolerance, 20.0f);
            Handles.DrawSolidArc(transform.position, Vector3.forward, normalizedDirection, -angleTolerance, 20.0f);
        }
    }
#endif
}
