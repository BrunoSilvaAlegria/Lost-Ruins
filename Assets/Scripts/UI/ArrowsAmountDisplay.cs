using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsAmountDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject[] displayObjects;

    ArrowsSystem arrowsAmount;
    float healthSystem;

    void Start()
    {
        
    }

    void Update()
    {

    }

    void UpdateHealth(int damage, Transform damageSource)
    {
        int health = 0;



        for (int i = 0; i < displayObjects.Length; i++)
        {
            displayObjects[i].SetActive(i < health);
        }
    }
}
