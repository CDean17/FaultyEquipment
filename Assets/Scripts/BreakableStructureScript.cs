using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableStructureScript : MonoBehaviour
{
    //parameters that can be changed about the object
    public bool anchored = false;
    public float health = 100f;
    public float deathTime = 0.2f;
    public float rayLengthX = 0.5f;
    public float rayLengthY = 0.5f;
    public float fallDamageMultiplier = 10f;
    public float minDarkenPercent = 25f;


    //private parameters
    private float maxHealth;
    public Rigidbody2D rb2d;
    private SpriteRenderer spr;
    public GameObject[] supports = new GameObject[4];
    private bool dead = false;
    public Vector3 rayOriginOffset = new Vector3(0.5f, 0.5f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        spr = transform.GetComponent<SpriteRenderer>();
        rb2d = transform.GetComponent<Rigidbody2D>();


        AddSupports();

    }

    void AddSupports()
    {

        RaycastHit2D[] hits = new RaycastHit2D[4];

        hits[0] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.up, rayLengthY);
        hits[1] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.right, rayLengthX);
        hits[2] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.down, rayLengthY);
        hits[3] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.left, rayLengthX);

        //if there is a raycast hit and it is a breakable tell it to add this object as a support 
        for (int i = 0; i < 4; i++)
        {

            if (hits[i].collider != null)
            {
                //check that the collided object is a breakable and is not the object that just added itself as a support
                if (hits[i].collider.TryGetComponent(out BreakableStructureScript b))
                {
                        supports[i] = hits[i].collider.gameObject;
                }
            }
        }

    }


    //Called when a neigboring object is destroyed. Checks whether this object is still supported using a recursive function. If it isn't destroy this object and repeat the process.
    private bool AlertConnected(GameObject g)
    {

        bool result = false;
        if (!anchored)
        {
            for (int i = 0; i < supports.Length; i++)
            {
                if (supports[i] != null && supports[i] != g)
                {
                    result = supports[i].GetComponent<BreakableStructureScript>().AlertConnected(gameObject);

                    if (result)
                    {
                        break;
                    }
                }
            }
        }
        else
        {
            result = true;
        }


        return result;
    }


    //called when an object is made not part of the structure. Tells all of its connected objects to make sure they are still supported
    private void CheckSupports(GameObject g)
    {
        bool result = false;
        foreach (GameObject game in supports)
        {
            if(game != null && game != g)
            {
                result = game.GetComponent<BreakableStructureScript>().AlertConnected(gameObject);
            }

            if (result)
            {
                break;
            }
        }

        if (!result)
            StartCoroutine(UnsupportObject(deathTime));

    }

    //Called by an object to damage this object
    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0 && !dead)
        {
            DestroyObj();
            dead = true;
        }
        float percentMax = (health / maxHealth);
        percentMax = (100.0f - minDarkenPercent) * percentMax;
        percentMax = percentMax + minDarkenPercent;
        percentMax = percentMax / 100f;
        spr.color = new Color(1 * percentMax, 1 * percentMax, 1 * percentMax, 1);
    }

    //Call this when you want a piece to become physics enabled
    IEnumerator UnsupportObject(float time)
    {
        yield return new WaitForSeconds(time);

        foreach (GameObject game in supports)
        {
            if (game != null)
            {
                 game.GetComponent<BreakableStructureScript>().CheckSupports(gameObject);
            }

        }

        supports = new GameObject[1];
        rb2d.bodyType = RigidbodyType2D.Dynamic;

    }


    //Call this to destroy the object
    void DestroyObj()
    {

        foreach (GameObject game in supports)
        {
            if (game != null)
            {
                game.GetComponent<BreakableStructureScript>().CheckSupports(gameObject);
            }

        }

        Destroy(gameObject);

    }

    //Called on a collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(rb2d.bodyType == RigidbodyType2D.Dynamic)
        {
            TakeDamage(Mathf.Abs(rb2d.velocity.y) * fallDamageMultiplier);

            if(collision.gameObject.TryGetComponent(out BreakableStructureScript b)){
                b.TakeDamage(Mathf.Abs(rb2d.velocity.y) * fallDamageMultiplier);
            }
        }
    }
}
