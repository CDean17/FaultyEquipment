using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickToExplode : MonoBehaviour
{
    public GameObject toSpawn;
    private Camera mainCam;
    private float coolDown = 1f;
    private float nextFire = 0f;
    // Start is called before the first frame update
    void Start()
    {
        mainCam = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.isPressed && nextFire < Time.time)
        {
            nextFire = coolDown + Time.time;
            Instantiate(toSpawn, mainCam.ScreenToWorldPoint(Mouse.current.position.ReadValue()), new Quaternion());
        }
        
    }
}
