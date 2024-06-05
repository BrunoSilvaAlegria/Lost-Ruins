using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeObjectStatic : MonoBehaviour
{
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        if(collisionInfo.gameObject.CompareTag("Ground")){
            transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
    }

}