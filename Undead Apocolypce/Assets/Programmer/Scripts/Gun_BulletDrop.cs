using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun_BulletDrop : MonoBehaviour
{
    class Bullet
    {
        public float time;
        public Vector3 initialPosistion;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;

    }



    //Gun stats
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots, impactForce, bulletSpeed, bulletDrop;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    float bulletMaxLifeTime = 5f;


    //bools 
    bool shooting, readyToShoot, reloading;

    [Space]
    //Reference
    public Camera fpsCam;
    public RaycastHit rayHit;
    Ray ray;
    public GameObject firePoint;
    List<Bullet> bullets = new List<Bullet>();



    public Transform raycastOrigin;
    public Transform raycastDestination;

    Vector3 GetPosition(Bullet bullet)
    {
        //Initalposition + Velocity*time + 0.5gravity * time * time
        Vector3 gravity = Vector3.down * bulletDrop;

        return (bullet.initialPosistion) + (bullet.initialVelocity * bullet.time) + (0.5f * gravity * bullet.time * bullet.time);
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosistion = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(bulletTracer, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        return bullet;
    }


    [Space]
    public float aimX;
    public float aimY;
    public float aimZ;

    public float orgPosX;
    public float orgPosY;
    public float orgPosZ;

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
    }
    private void Update()
    {
        PlayerInput();
        UpdateBullet(Time.deltaTime);

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
        }
        else
        {
            // Debug.Log("Not aiming");
            gameObject.transform.localPosition = new Vector3(orgPosX, orgPosY, orgPosZ);

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

    public void UpdateBullet(float deltaTime)
    {
        SimulateBullets(deltaTime);
    }

    void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);

        });
    }

    void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time > bulletMaxLifeTime);
    }



    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        // Spread
        //Calculate Direction with Spread
        Vector3 deviation3D = Random.insideUnitCircle * spread;
        Quaternion rot = Quaternion.LookRotation(Vector3.forward * range + deviation3D);
        Vector3 forwardVector = firePoint.transform.rotation * rot * Vector3.forward;

        Vector3 direction = end - start;
        ray.origin = start;
        ray.direction = end;


        if (Physics.Raycast(firePoint.transform.position,raycastDestination.position, out rayHit, range))
        {
            Debug.DrawLine(firePoint.transform.position, rayHit.point, Color.red, 1.0f);
            //var tracer = Instantiate(bulletTracer, firePoint.transform.position, Quaternion.identity);
           bullet.tracer.AddPosition(firePoint.transform.position);

            Debug.Log(rayHit.collider.name);
            //Plays the muzzleflash and Hiteffect
            muzzleFlash.Play();
            hitEffect.transform.position = rayHit.point;
            hitEffect.transform.forward = rayHit.normal;
            hitEffect.Emit(5);

            bullet.tracer.transform.position = rayHit.point;
            bullet.time = bulletMaxLifeTime;

            if (rayHit.collider.CompareTag("Enemy"))
            {


                //Setup damage Function
                Debug.Log("Hit Enemy");
                TargetHealth target = rayHit.transform.GetComponentInParent<TargetHealth>(); //Referencing the TargetHealth script
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
        else
        {
            bullet.tracer.transform.position = end;
        }


    }



    private void Shoot()
    {
        readyToShoot = false;

        Vector3 velocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;
        var bullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Add(bullet);

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
