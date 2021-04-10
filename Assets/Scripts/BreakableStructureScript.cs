using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableStructureScript : MonoBehaviour
{
    //parameters that can be changed about the object
    public bool anchored = false;
    public float health = 100f;
    public float deathTime = 0.2f;
    public float rayLength = 0.5f;
    public float fallDamageMultiplier = 10f;
    public float minDarkenPercent = 25f;


    //private parameters
    private float maxHealth;
    private Rigidbody2D rb2d;
    private SpriteRenderer spr;
    public GameObject[] supports = new GameObject[4];
    private bool populatedSupports = false;
    private bool dead = false;
    private Vector3 rayOriginOffset;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        spr = transform.GetComponent<SpriteRenderer>();
        rb2d = transform.GetComponent<Rigidbody2D>();

        rayOriginOffset = new Vector3(0.5f, 0.5f, 0f);

        //do ray checks in each cardinal direction to retrieve supporting objects.   
        //NOTE: in this scheme for both supports and hits arrays 0 is up, 1 is right, 2 bottom, 3 left
        RaycastHit2D[] hits = new RaycastHit2D[4];
        if (anchored)
        {
            hits[0] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.up, rayLength);
            hits[1] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.right, rayLength);
            hits[2] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.down, rayLength);
            hits[3] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.left, rayLength);

            //if there is a raycast hit and it is a breakable tell it to add this object as a support 
            for (int i = 0; i < 4; i++)
            {

                if (hits[i].collider != null)
                {
                    //check that the collided object is a breakable and is not the object that just added itself as a support
                    if (hits[i].collider.TryGetComponent(out BreakableStructureScript b))
                    {
                            b.AddSupports(gameObject, i);
                            supports[i] = hits[i].collider.gameObject;
                    }
                }
            }
            populatedSupports = true;
        }


    }

    void AddSupports(GameObject g, int direction)
    {

        //add the support that was told to be added and from the correct direction
        //Stuarts wizard code replacing the switch statement
        direction = direction + 2 > 3 ? direction - 2 : direction + 2;
        supports[direction] = g;

        //if this object is now supported (and it should if everything went right) tell other objects near it to add is as a support
        RaycastHit2D[] hits = new RaycastHit2D[4];

        hits[0] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.up, rayLength);
        hits[1] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.right, rayLength);
        hits[2] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.down, rayLength);
        hits[3] = Physics2D.Raycast(transform.position + rayOriginOffset, Vector2.left, rayLength);

        //if there is a raycast hit and it is a breakable tell it to add this object as a support 
        for (int i = 0; i < 4; i++)
        {

            if (hits[i].collider != null)
            {
                //check that the collided object is a breakable and is not the object that just added itself as a support
                if (hits[i].collider.TryGetComponent(out BreakableStructureScript b) && i != direction)
                {
                        b.AddSupports(gameObject, i);
                        supports[i] = hits[i].collider.gameObject;
                }
            }
        }

        //set this variable to true so the object knows it doesn't need to update other nearby objects again
        populatedSupports = true;

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
            StartCoroutine(DestroyAftertime(deathTime));
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
    IEnumerator DestroyAftertime(float time)
    {
        yield return new WaitForSeconds(time);

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
                b.TakeDamage(Mathf.Abs(rb2d.velocity.x) * fallDamageMultiplier);
            }
        }
    }
}
