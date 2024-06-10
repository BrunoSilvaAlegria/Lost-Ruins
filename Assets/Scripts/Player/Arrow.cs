using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float arrowSpeed = 20f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int damage = 1;
    [SerializeField] GameObject enemy;


    public void SetDirection(Vector2 direction)
    {
        rb.velocity = direction * arrowSpeed;
        if (direction.x < 0)
        {
            //Flip the sprite horinzontally if moving left
            transform.localScale = new Vector3(-1,transform.localScale.y, transform.localScale.z);
        }
        else
        {
            //Don't flip the sprite if moving right
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }

    void Update()
    {
 
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        HealthSystem enemyHealth = hitInfo.GetComponent<HealthSystem>();
        if (enemyHealth != null)
        {
            enemyHealth.DealDamage(damage, enemy.transform);
            //Debug.Log ($"{gameObject.name} hit");
        }
        Destroy(gameObject);
    }
}
