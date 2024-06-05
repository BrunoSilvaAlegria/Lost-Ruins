using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollow : MonoBehaviour
{
    [SerializeField]
    private Transform objectToFollow;
    [SerializeField]
    private float     speed = 0.5f;

    void FixedUpdate()
    {
        if (objectToFollow != null)
        {
            Vector3 targetPosition = objectToFollow.position;

            targetPosition.z = transform.position.z;

            Vector3 delta = targetPosition - transform.position;

            transform.position = transform.position + delta * speed;

        }
    }
}
  