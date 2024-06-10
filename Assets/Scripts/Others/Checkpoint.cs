using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private ParticleSystem onPS;
    [SerializeField] private ParticleSystem offPS;
    bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        EnableCheckpoint(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnableCheckpoint(bool state)
    {
   
        active = state;
        //Debug.Log($"Checkpoint {this} enable state: {active}");

        if (onPS)
        {
            var emissionModule = onPS.emission;
            emissionModule.enabled = active;
        }
        if (offPS)
        {
            var emissionModule = offPS.emission;
            emissionModule.enabled = !active;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Player player = collider.GetComponent<Player>();
        if (player == null) player = collider.GetComponentInParent<Player>();
        if (player != null)
        {
            LevelManager.instance.SetCheckpoint(this);
        }
    }
}