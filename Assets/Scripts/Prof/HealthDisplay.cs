using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject[] displayObjects;

    HealthSystem healthSystem;

    void Start()
    {
        
    }

    void Update()
    {
        if (healthSystem == null)
        {
            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                healthSystem = player.GetComponent<HealthSystem>();
                healthSystem.onDamage += UpdateHealth;
                UpdateHealth(0, null);
            }
        }
    }

    void UpdateHealth(int damage, Transform damageSource)
    {
        int health = 0;

        if (healthSystem != null)
        {
            health = healthSystem.health;
        }

        for (int i = 0; i < displayObjects.Length; i++)
        {
            displayObjects[i].SetActive(i < health);
        }
    }
}
