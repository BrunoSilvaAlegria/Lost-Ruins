using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //ATTACK
    [Header("Bullet Damage")]
    [SerializeField] private float bulletDamage = 1;

    private void OnCollisionStay2D(Collision2D collision)
    {
        //var healthController = collision.gameObject.GetComponent<HealthController>();

        //healthController.TakeDamage(bulletDamage);
    }
}
