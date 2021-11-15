using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public PlayerHealth targethealth;
    public Slider Healthbar;

    public void SetMaxHealth(float health)
    {
        Healthbar.maxValue = health;
        Healthbar.value = health;

    }



    public void SetHealth(float health)
    {
        Healthbar.value = health;
    }
}
