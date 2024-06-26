using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    [SerializeField] private GameObject playerDetectionObject;
    [SerializeField] private AudioSource hitGround;

    private Rigidbody2D rb;
    private BoxCollider2D bx;
    private bool grounded;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bx = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Player"))
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 7;
            rb.mass = 400;
            bx.enabled = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            rb.bodyType = RigidbodyType2D.Static;
            hitGround.Play();
        }
        if (!grounded)
        {
            grounded = true;
            this.enabled = false;
        }
    }
}