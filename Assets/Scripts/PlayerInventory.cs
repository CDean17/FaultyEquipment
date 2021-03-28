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
    void Start()
    {
        slots = new List<GameObject>(inventorySize);

        for(int i=0; i<inventorySize; i++)
        {
            slots.Add(Instantiate(defaultObject, transform));
            //slots.Add(new GameObject());
            //slots[i].transform.parent = gameObject.transform;
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
        //for test purposes just making a new gameobject and adding it
        slots[selectedSlot] = new GameObject();
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
}
