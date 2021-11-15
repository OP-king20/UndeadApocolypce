using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
   

    //bool
    private bool isAiming;

    //Scripts
    Gun gun_script;
    public static Recoil instance;

    //Rotations
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    
    // Update is called once per frame
    void Update()
    {
        gun_script = GetComponentInChildren<Gun>();

        //Gun.instance.aiming = isAiming;

        isAiming = gun_script.aiming;

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, gun_script.returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, gun_script.snappiness * Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(currentRotation);

    }

    public void RecoilFire()
    {

        if (isAiming)
        {

            targetRotation += new Vector3(gun_script.aimRecoilX, Random.Range(-gun_script.aimRecoilY, gun_script.aimRecoilY), Random.Range(-gun_script.aimRecoilZ, gun_script.aimRecoilZ));
        }
        else
        {
            targetRotation += new Vector3(gun_script.recoilX, Random.Range(-gun_script.recoilY, gun_script.recoilY), Random.Range(-gun_script.recoilZ, gun_script.recoilZ));
        }
    }



}
