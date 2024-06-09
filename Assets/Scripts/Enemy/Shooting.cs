using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shooting : MonoBehaviour
{
    //ATTACK
    [Header("Enemy Shots")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform gunPos;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private UnityEvent<GameObject> onPlayerDetected;
    [Range(0.1f, 1f)] public float detectionRadius;
    [SerializeField] private float shootInterval = 2.0f;

    //Gizmos for ATTACK
    [Header("Gizmos parameters")]
    [SerializeField] private Color color = Color.green;
    [SerializeField] private bool showGizmos = true;

    public bool playerDetected { get; private set; }

    private float timer;
    private bool isFacingRight = true;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player == null) player = collision.GetComponentInParent<Player>();

        if (player != null)
        {
            timer += Time.deltaTime;
            if (timer > shootInterval)
            {
                timer = 0;
                Shoot();
            }
        }
    }

    private void Update()
    {
        var collider = Physics2D.OverlapCircle(transform.position, detectionRadius, targetLayer);
        playerDetected = collider != null;
    }
    private void Shoot()
    {
        //ANIMATION
        anim.SetBool("SHOOTING", true);

        GameObject bulletObject = Instantiate(bulletPrefab, gunPos.position, gunPos.rotation);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.SetDirection(isFacingRight ? Vector2.right : Vector2.left);
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, detectionRadius);
        }
    }
}
