using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] 
    private float speed = 150;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundCheckRadius = 2;
    [SerializeField]
    private LayerMask groundCheckLayers;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private float wallCheckRadius = 2;
    [SerializeField]
    private LayerMask wallCheckLayers;

    private Rigidbody2D rb;
    private HealthSystem hs;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            hs = player.GetComponent<HealthSystem>();
            if (hs != null)
            {
                hs.onDeath += PlayerDied;
            }
        }
    }

    private void OnDestroy()
    {
        if (hs)
        {
            hs.onDeath -= PlayerDied;
        }
    }

    void PlayerDied()
    {
        speed = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if ((!HasGroundAhead()) || (HasWallAhead()))
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
        }

        Vector2 currentVelocity = rb.velocity;

        currentVelocity.x = transform.right.x * speed;

        rb.velocity = currentVelocity;
    }

    bool HasGroundAhead()
    {
        Collider2D collider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundCheckLayers);

        return collider != null;
    }
    bool HasWallAhead()
    {
        Collider2D collider = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallCheckLayers);

        return collider != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = new Color(1.0f, 0.5f, 0.0f, 0.5f);
            Gizmos.DrawSphere(groundCheck.position, groundCheckRadius);
        }
        if (wallCheck)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(wallCheck.position, wallCheckRadius);
        }
    }
}
