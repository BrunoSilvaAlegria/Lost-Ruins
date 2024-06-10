using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    [SerializeField] private GameObject playerDetectionObject;

    private Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collisionInfo)
    {
        if(collisionInfo.gameObject.CompareTag("Player"))
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 7;
            rb.mass = 400;
        }
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
}