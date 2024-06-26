using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private int damage = 1;
    [SerializeField] GameObject player;


    public void SetDirection(Vector2 direction)
    {
        rb.velocity = direction * bulletSpeed;
        if (direction.x < 0)
        {
            //Flip the sprite horinzontally if moving left
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            //Don't flip the sprite if moving right
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        HealthSystem playerHealth = hitInfo.GetComponent<HealthSystem>();
        if (playerHealth != null)
        {
            playerHealth.DealDamage(damage, player.transform);
            Destroy(gameObject);
        }
    }
}