using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //ATTACK
    [Header("Enemy Shots")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletPos;

    private float timer;
    private HealthSystem health;
    private Animator anim;

    
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 2) 
        {
            timer = 0; 
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
    }

    public void TakeDamage(int dmg)
    {
        //Enemy takes damage, reduces health
        //dmg = 
        //currentHealth -= dmg;

        anim.SetTrigger("HURT"); //Damage animation

        //if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Enemy dead");

        //Death animation
        anim.SetBool("DEATH", true);

        //Disable enemy
        this.enabled = false;
    }
}
