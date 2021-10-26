using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MeleeSystem : MonoBehaviour
{
    public int attackDamage, ultAttackDamage;
    public float attackrange, impactforce, timebetweenattack, ultTimeBetweenAttack, staminaUse, ultStaminaUse;

    //bools 
    bool Canattack;

    //Reference
    public Camera fpsCam;
    public RaycastHit rayHit;


    //Graphics
    public GameObject impactGraphic;
    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        Canattack = true;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();

        //SetText to display bullets left
        text.SetText(gameObject.name);
    }

    void PlayerInput()
    {

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward;

        if (Input.GetKeyDown(KeyCode.Mouse0) && Canattack == true && StaminaBar.instance.currentStamina >= 0.1f)
        {
            Debug.Log("normal attack");
            Canattack = false;
            //Vet inte om det är bra att använda raycasts i ett melee system, används just nu tills en bättre lösning finns.
            //RayCast
            if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, attackrange))
            {
                Debug.Log(rayHit.collider.name);
                StaminaBar.instance.UseStamina(staminaUse);

                if (rayHit.collider.CompareTag("Enemy"))
                {
                    TargetHealth target = rayHit.transform.GetComponent<TargetHealth>();
                    //Setup damage Function
                    Debug.Log("Hit Enemy");
                    if (target != null)
                    {
                        target.TakeDamage(attackDamage);
                    }

                    if (rayHit.rigidbody != null)
                    {
                        rayHit.rigidbody.AddForce(-rayHit.normal * impactforce);
                    }

                }

                //Graphics
                Instantiate(impactGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
               
            }

            Invoke("Resetattack", timebetweenattack);
        }
        else if(Input.GetKeyDown(KeyCode.Mouse1) && Canattack == true && StaminaBar.instance.currentStamina >= 0.1f)
        {
            Debug.Log("Hard Attack");
            Canattack = false;
            //Vet inte om det är bra att använda raycasts i ett melee system, används just nu tills en bättre lösning finns.
            //RayCast
            if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, attackrange))
            {
                Debug.Log(rayHit.collider.name);
                StaminaBar.instance.UseStamina(ultStaminaUse);

                if (rayHit.collider.CompareTag("Enemy"))
                {
                    TargetHealth target = rayHit.transform.GetComponent<TargetHealth>();
                    //Setup damage Function
                    Debug.Log("Hit Enemy");
                    if (target != null)
                    {
                        target.TakeDamage(ultAttackDamage);
                    }

                    if (rayHit.rigidbody != null)
                    {
                        rayHit.rigidbody.AddForce(-rayHit.normal * impactforce);
                    }

                }

                //Graphics
                Instantiate(impactGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));

            }

            Invoke("Resetattack", ultTimeBetweenAttack);

        }
        else if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) && StaminaBar.instance.currentStamina <= 0f)
        {
            Debug.Log("Cant perform attack");
            Canattack = false;
        }
    }


    private void Resetattack()
    {
        Canattack = true;
    }
}
