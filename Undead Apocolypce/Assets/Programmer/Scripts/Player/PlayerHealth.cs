using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100f;
    public float maxHealth = 100f;
  
    public HealthBar Hb;
    public static PlayerHealth intsanse;
    public bool TakingDamage;

    private WaitForSeconds regenTime = new WaitForSeconds(0.01f);
    private Coroutine regen;


    private void Start()
    {
        health = maxHealth;
        Hb.SetMaxHealth(maxHealth);

    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        TakingDamage = true;

        Hb.SetHealth(health);

        if (health <= 0)
        {
            Die();

        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    // Needs a rework so you can regenerate Health when not attacked. 

    public void Regen()
    {
        if (health >= 0)
        {

            //if (regen != null)
            //{
            //    StopCoroutine(regen);
            //    Debug.Log("Test");
            //}
            regen = StartCoroutine(RegenStamina());
            Debug.Log("Start");


        }
        else
        {
            Debug.Log("Not enough stamina");

        }

    }


    public IEnumerator RegenStamina()
    {

        yield return new WaitForSeconds(2);

        while (health <= maxHealth)
        {
            health += maxHealth / 100;
            Debug.Log("Regenerate");
            yield return regenTime;
        }
        //regen = null;
    }



    //private IEnumerator RegenStamina()
    //{
    //    yield return new WaitForSeconds(2);

    //    while (health < maxHealth)
    //    {
    //        health += maxHealth / 100;
    //        yield return regenTime;
    //    }
    //    regen = null;
    //}
}
