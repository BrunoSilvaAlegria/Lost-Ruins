using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    //[SerializeField] int health; //referenciar o player health 
    //[SerializeField] Image healthBar;
    [SerializeField] Slider slider;

   // private float maxHealth = 4f;

    public void SetHealth(int health)
    {
        slider.value = health;
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
}
