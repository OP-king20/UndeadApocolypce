using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public TargetHealth targethealth;
    public Slider Healthbar;
    public float _health;
    public float _maxHealth;


    public static HealthBar instance;

   

    private void Awake()
    {
        instance = this;


    }

    // Start is called before the first frame update
    void Start()
    {
        //Making sure the staminabar is set at 100 at the start of the game
        Healthbar.maxValue = targethealth.maxHealth;
        Healthbar.value = targethealth.maxHealth;
    }

    private void Update()
    {
  
        Healthbar.value = targethealth.health;

        //if(targethealth.health < targethealth.maxHealth)
        //{

        //    targethealth.Regen();
        //}


    }
}
