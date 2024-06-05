using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth = 6f;

    public float health { get { return currentHealth/maxHealth; } }

    public void TakeDamage (float damage)
    {
        if (currentHealth < 0) return;

        currentHealth -= damage;
        if (currentHealth <= 0) currentHealth = 0;


    }

}
