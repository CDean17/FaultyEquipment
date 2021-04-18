using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponScript : MonoBehaviour
{
    private GameObject attachedPlayer;
    public GameObject bulletSpawn;
    public GameObject spawnedProjectile;
    private SpriteRenderer spr;
    public Vector2 aimDirection;
    private bool flipped = false;
    public bool aiming = false;
    public bool releasedAim = false;
    private float nextFire = 0;

    //weapon properties
    public float aimSpeed = 75f;
    public float fireDelay = 0.5f;
    public float firingArc = 0f;
    public int projectilesToFire = 1;
    public int totalAmmo = 50;
    

    // Start is called before the first frame update
    void Start()
    {
        attachedPlayer = transform.parent.gameObject;
        spr = transform.GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        //set mirroring
        if(attachedPlayer.GetComponent<twodPlayerMovement>().moveDirection < 0 && flipped == false)
        {
            
            flipped = true;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            spr.flipY = true;

        }
        else if(attachedPlayer.GetComponent<twodPlayerMovement>().moveDirection > 0 && flipped == true)
        {
            
            flipped = false;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            spr.flipY = false;

        }

        if (releasedAim)
        {
            if (flipped)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            releasedAim = false;
        }

        if (aiming)
        {
            if (flipped)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, (transform.rotation.eulerAngles.z - (Time.deltaTime * aimDirection.y * aimSpeed))));
            }
            else
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, (transform.rotation.eulerAngles.z + (Time.deltaTime * aimDirection.y * aimSpeed))));
            }

        }
    }

    private void FixedUpdate()
    {
 


        //set aiming to false after we process assuming it is true, On aim will set it to true again next execution
        //aiming = false;
    }


    public void Fire()
    {
        
        if(nextFire < Time.time)
        {
            for (int i = 0; i < projectilesToFire;  i++)
            {
                Quaternion fireAngle = bulletSpawn.transform.rotation * Quaternion.Euler(new Vector3(0, 0, Random.Range(-(firingArc / 2), (firingArc / 2))));
                Instantiate(spawnedProjectile, bulletSpawn.transform.position, fireAngle);
            }

            nextFire = Time.time + fireDelay;
            totalAmmo--;
        }
        

    }

}
