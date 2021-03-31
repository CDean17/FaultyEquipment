using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> slots;
    public int selectedSlot = 0;
    private int lastSlot = 0;
    public int inventorySize = 3;
    public GameObject defaultObject;
    public GameObject collidingWep;
    void Start()
    {
        slots = new List<GameObject>(inventorySize);

        for(int i=0; i<inventorySize; i++)
        {
            slots.Add(Instantiate(defaultObject, transform));
            slots[selectedSlot].transform.position = transform.position;

        }
        for(int i = 1; i<inventorySize; i++)
        {
            slots[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPickup()
    {
        if(collidingWep != null)
        {
            GameObject g = slots[selectedSlot];
            GameObject.Destroy(g);
            slots[selectedSlot] = Instantiate(collidingWep.GetComponent<PickupScript>().weapon, transform);
            slots[selectedSlot].transform.position = transform.position;
            GameObject.Destroy(collidingWep);
        }
        
    }

    public void OnSelectSlot(InputValue input)
    {
       
        Vector2 v = input.Get<Vector2>();

        if (v.x == -1)
        {
            slots[selectedSlot].SetActive(false);
            selectedSlot = 0;
            slots[selectedSlot].SetActive(true);
        }
        else if(v.y == 1)
        {
            slots[selectedSlot].SetActive(false);
            selectedSlot = 1;
            slots[selectedSlot].SetActive(true);
        }
        else if(v.x == 1)
        {
            slots[selectedSlot].SetActive(false);
            selectedSlot = 2;
            slots[selectedSlot].SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("pickup"))
        {
            collidingWep = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("pickup"))
        {
            collidingWep = null;
        }
    }

    public void OnMove(InputValue input)
    {
        slots[selectedSlot].GetComponent<WeaponScript>().aimDirection = input.Get<Vector2>();
    }

    public void OnFire()
    {
        slots[selectedSlot].GetComponent<WeaponScript>().Fire();
    }

    public void OnAim(InputValue input)
    {
        if(input.Get<float>() == 0)
        {
            slots[selectedSlot].GetComponent<WeaponScript>().aiming = false;
            slots[selectedSlot].GetComponent<WeaponScript>().releasedAim = true;
        }
        else
        {
            slots[selectedSlot].GetComponent<WeaponScript>().aiming = true;
        }
        
        //Debug.Log("Aiming!");
    }

    public void OnAimCancelled()
    {
        slots[selectedSlot].GetComponent<WeaponScript>().aiming = false;
    }
}
