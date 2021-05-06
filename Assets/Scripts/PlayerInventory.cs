using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInventory : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> slots;
    public int selectedSlot = 0;
   // private int lastSlot = 0;
    public int inventorySize = 3;
    public GameObject defaultObject;
    public GameObject collidingWep;
    private GameObject collidingBuildingObj;

    //Health related variables
    public float maxHealth = 100f;
    public float currentHealth;
    public GameObject deadPlayerObj;
    public GameObject foreverBox;
    private Rigidbody2D rb2d;
    private SpriteRenderer spr;
    public bool dead = false;

    //Gamemanagement Variables
    private GameObject gameManager;
    public GameObject uIMain;
    public int playerNumber;
    private Color pColor;

    public float timeTillUpdateGame = 0.5f;
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //get access to gameManager
        gameManager = GameObject.FindGameObjectWithTag("GameController");

        //Set color based on number of existing players
        spr = transform.GetComponent<SpriteRenderer>();
        playerNumber = GameObject.FindGameObjectsWithTag("Player").Length;

        switch (playerNumber)
        {
            case 1:
                pColor = spr.color;
                break;
            case 2:
                pColor = new Color(0.7f, 0.3f, 0.3f);
                break;
            case 3:
                pColor = new Color(0.7f, 0.7f, 0.3f);
                break;
            case 4:
                pColor = new Color(0.3f, 0.3f, 0.7f);
                break;
        }

        spr.color = pColor;


        rb2d = transform.GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

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
        //Code for picking up weapons
        if(collidingWep != null)
        {
            GameObject g = slots[selectedSlot];
            GameObject.Destroy(g);
            slots[selectedSlot] = Instantiate(collidingWep.GetComponent<PickupScript>().weapon, transform);
            slots[selectedSlot].transform.position = transform.position;
            GameObject.Destroy(collidingWep);
        }


        //Code for interacting with objects
        if (collidingBuildingObj != null)
        {
            
            if(collidingBuildingObj.TryGetComponent(out DoorScript b))
            {
                b.ToggleOpen();
            }
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

        if (collision.gameObject.CompareTag("building"))
        {
            collidingBuildingObj = collision.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("building"))
        {
            collidingBuildingObj = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("pickup"))
        {
            collidingWep = null;
        }
        if (collision.gameObject.CompareTag("building"))
        {
            collidingBuildingObj = null;
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

    public void OnBack()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "LobbyScene")
        {
            gameManager.GetComponent<GameManagerScript>().dropPlayer(gameObject);
            Destroy(gameObject);
        }
    }

    public void OnStart()
    {
        gameManager.GetComponent<GameManagerScript>().initializeGame();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        uIMain.GetComponent<PlayerUIScript>().UpdateHealthbar(currentHealth/maxHealth);

        if (currentHealth <= 0f)
        {
            GameObject b = Instantiate(deadPlayerObj, transform.position, transform.rotation);
            b.GetComponent<SpriteRenderer>().color = pColor;
            rb2d.velocity = Vector3.zero;
            transform.position = foreverBox.transform.position;
            dead = true;

            
            StartCoroutine(DeathUpdate(timeTillUpdateGame));
        }
    }

    IEnumerator DeathUpdate(float time)
    {
        yield return new WaitForSeconds(time);
        
        gameManager.GetComponent<GameManagerScript>().checkAlive();
        
    }

    private void OnSceneLoaded(Scene aScene, LoadSceneMode aMode)
    {

        switch (playerNumber)
        {
            case 1:
                uIMain = GameObject.Find("Player1UI");
                break;
            case 2:
                uIMain = GameObject.Find("Player2UI");
                spr.color = new Color(0.7f, 0.3f, 0.3f);
                break;
            case 3:
                uIMain = GameObject.Find("Player3UI");
                spr.color = new Color(0.7f, 0.7f, 0.3f);
                break;
            case 4:
                uIMain = GameObject.Find("Player4UI");
                spr.color = new Color(0.3f, 0.3f, 0.7f);
                break;
        }

        foreverBox = GameObject.FindGameObjectWithTag("deathbox");
        
    }

    public void resetInventory()
    {
        uIMain.GetComponent<PlayerUIScript>().UpdateHealthbar(1);
        uIMain.GetComponent<PlayerUIScript>().UIReset();
        //reset inventory
        for (int i = 0; i < inventorySize; i++)
        {
            Destroy(slots[selectedSlot]);

        }
        slots = new List<GameObject>(inventorySize);
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(Instantiate(defaultObject, transform));
            slots[selectedSlot].transform.position = transform.position;

        }
        for (int i = 1; i < inventorySize; i++)
        {
            slots[i].SetActive(false);
        }
    }


}
