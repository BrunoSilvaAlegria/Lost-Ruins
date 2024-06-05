using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    private bool grounded;

    void Start()
    {
        
    }

    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D collisionInfo) 
    {
        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            if (!grounded)
            {
                grounded = true;
            }
        }   
    }  
}
