using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite spr;
    public GameObject weapon;
    public WeaponScriptableObjects scriptObj;
    public int currentAmmo = 10;
    public bool setStartAmmo = true;


    void Start()
    {
        //sets current ammo equal to the weapons objects ammo
        if (setStartAmmo)
        {
            currentAmmo = weapon.GetComponent<WeaponScript>().totalAmmo;
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
