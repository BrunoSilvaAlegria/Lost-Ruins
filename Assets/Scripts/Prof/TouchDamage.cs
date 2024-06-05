using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchDamage : MonoBehaviour
{
    [SerializeField] private Faction faction;
    [SerializeField] private int damage = 1;
    [SerializeField] private bool destroyOnDamage = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        HealthSystem hs = collider.GetComponent<HealthSystem>();
        if (hs == null) hs = collider.GetComponentInParent<HealthSystem>();
        if (hs != null)
        {
            if (faction.IsHostile(hs.faction))
            {
                if (hs.DealDamage(damage))
                {
                    Debug.Log($"Collided with something with health system - {hs.name}");
                    if (destroyOnDamage)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
