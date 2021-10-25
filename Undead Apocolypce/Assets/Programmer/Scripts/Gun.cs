using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    /*TODO
     * Fix spread so it also gives some variation over the X- axis, also add a recoil force when fireing. 
     * Add higher spread when moving/jumping
     * 
     */



    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots, impactForce;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;


    //bools 
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public RaycastHit rayHit;

    //Graphics
    public GameObject bulletHoleGraphic;
    public ParticleSystem muzzleFlash;
    public CameraShake CameraShake;
    public float camShakeMagnitude, camShakeDuration;
    public TextMeshProUGUI text;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
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

        //Aiminn system
        if (Input.GetKey(KeyCode.Mouse1))
        {
           // Debug.Log("aiming");
            gameObject.transform.localPosition = new Vector3(0f, -0.1f, 0.8f);
        }
        else
        {
           // Debug.Log("Not aiming");
            gameObject.transform.localPosition = new Vector3(0.35f, -0.25f, 0.6f);

        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

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

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        //if(PlayerMovement.instance.controller.velocity.magnitude > 0)
        //{
        //    spread = spread * 1.5f;
        //}

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range))
        {
            Debug.Log(rayHit.collider.name);
            //Plays the muzzleflash
            muzzleFlash.Play();

            if (rayHit.collider.CompareTag("Enemy"))
            {

                
                //Setup damage Function
                Debug.Log("Hit Enemy");
                TargetHealth target = rayHit.transform.GetComponent<TargetHealth>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }

                if (rayHit.rigidbody != null)
                {
                    rayHit.rigidbody.AddForce(-rayHit.normal * impactForce);
                }
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
