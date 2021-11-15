using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Author: Samuel Pettersson
// * Unity Version: 2020.3.16
// * IDE: Visual Studio Community 2017
// * Date: November 2021
// * Instructions: This script will add Gun mechanics

public class Gun : MonoBehaviour
{

    /*TODO
     * Fix spread so it also gives some variation over the X- axis, also add a recoil force when fireing. 
     * Add higher spread when moving/jumping
     * 
     */

    public static Gun instance;

    //Gun stats
    public float damage;
    float appliedDmg;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots, impactForce;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools 
    bool shooting, readyToShoot,reloading;
    public bool aiming;

    [Space]
    //Reference
    public Camera fpsCam;
    public RaycastHit rayHit;
    public GameObject firePoint;

    //Recoil varables
    private Recoil recoil_script;


    [Space]
    public float aimX;
    public float aimY;
    public float aimZ;

    public float orgPosX;
    public float orgPosY;
    public float orgPosZ;

    //Hipfire Recoil
    [Space]
    [Header("Hipfire Recoil")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;


    //Aim Recoil
    [Space]
    [Header("Aim Recoil")]
    public float aimRecoilX;
    public float aimRecoilY;
    public float aimRecoilZ;

    //Recoil Settings
    [Space]
    [Header("Recoil Settings")]
    public float snappiness;
    public float returnSpeed;

    
    [Space]
    //Graphics
    public GameObject bulletHoleGraphic;
    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer bulletTracer;
    public CameraShake CameraShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI text;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;

        recoil_script = GetComponentInParent<Recoil>();
        
    }
    private void Update()
    {
        PlayerInput();
        //SetText to display bullets left
        text.SetText(bulletsLeft + " / " + magazineSize);

    }
    private void PlayerInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //TEMP AIM SYSTEM
        //Aiminn system
        if (Input.GetKey(KeyCode.Mouse1))
        {
            // Debug.Log("aiming");
            gameObject.transform.localPosition = new Vector3(aimX, aimY, aimZ);
            aiming = true;
        }
        else
        {
            // Debug.Log("Not aiming");
            gameObject.transform.localPosition = new Vector3(orgPosX, orgPosY, orgPosZ);
            aiming = false;
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
            Reload();

        //Shoot
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }
    private void Shoot()
    {
        readyToShoot = false;

        recoil_script.RecoilFire();

        //dmg = null;

        //Spread
        //Calculate Direction with Spread
        Vector3 deviation3D = Random.insideUnitCircle * spread;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward * range + deviation3D);
        Vector3 forwardVector = firePoint.transform.rotation * rot * Vector3.forward;

        //RayCast
        if (Physics.Raycast(firePoint.transform.position, -forwardVector, out rayHit, range))
        {
            Debug.DrawLine(firePoint.transform.position, rayHit.point, Color.red, 1.0f);
            var tracer = Instantiate(bulletTracer, firePoint.transform.position, Quaternion.identity);
            tracer.AddPosition(firePoint.transform.position);

            //Debug.Log(rayHit.collider.name);
            //Plays the muzzleflash and Hiteffect
            muzzleFlash.Play();
            hitEffect.transform.position = rayHit.point;
            hitEffect.transform.forward = rayHit.normal;
            hitEffect.Emit(5);

            tracer.transform.position = rayHit.point;

            if (rayHit.collider.CompareTag("Enemy"))
            {
                //Setup damage Function
                Debug.Log("Hit Enemy");
                
                TargetHealth target = rayHit.transform.GetComponentInParent<TargetHealth>(); //Referencing the TargetHealth script
                DamageZone dmg = rayHit.collider.GetComponent<DamageZone>();




                switch (dmg.hitbox)
                {
                    case DamageZone.Hitboxes.Head:
                        appliedDmg = damage * 1.1f;
                        target.TakeDamage(appliedDmg);
                        Debug.Log("Hit head " + "damage applied " + damage * 1.1f);
                        print(dmg.hitbox);
                        break;

                    case DamageZone.Hitboxes.Body:
                        appliedDmg = damage * 0.8f;
                        target.TakeDamage(appliedDmg);
                        Debug.Log("Hit Body " + "damage applied " + damage * 0.8f);
                        print(dmg.hitbox);
                        break;

                    case DamageZone.Hitboxes.Limbs:
                        appliedDmg = damage * 0.4f;
                        target.TakeDamage(appliedDmg);
                        Debug.Log("Hit Limb " + "damage applied " + damage * 0.4f);
                        print(dmg.hitbox);
                        break;
                }




                //if (target != null)
                //{

                //    dmg = rayHit.transform.GetComponent<DamageZone>();






                //    //target.TakeDamage(damage);
                //}


                //if (rayHit.rigidbody != null)
                //{
                //    rayHit.rigidbody.AddForce(-rayHit.normal * impactForce);
                //}
            }
        }

        //ShakeCamera
        CameraShake.Shake(camShakeDuration, camShakeMagnitude);

        //Graphics
        var tempbullet = Instantiate(bulletHoleGraphic, rayHit.point, Quaternion.LookRotation(rayHit.normal));
        tempbullet.transform.parent = rayHit.transform;

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }

   

    private void ResetShot()
    {
        readyToShoot = true;
    }
    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }
    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
