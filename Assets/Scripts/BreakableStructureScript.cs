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


    //private parameters
    public GameObject[] supports = new GameObject[4];
    public GameObject[] supporting = new GameObject[4];
    public bool supported = false;
    private bool populatedSupports = false;
    private bool dead = false;
    private Vector3 rayOriginOffset;

    // Start is called before the first frame update
    void Start()
    {
        rayOriginOffset = new Vector3(0.5f, 0.5f, 0f);
        //If the object is set as an anchor it is automatically set as supported
        if (anchored)
            supported = true;

        //do ray checks in each cardinal direction to retrieve supporting objects.   
        //NOTE: in this scheme for both supports and hits arrays 0 is up, 1 is right, 2 bottom, 3 left
        RaycastHit2D[] hits = new RaycastHit2D[4];
        if (supported)
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
                        if (!b.populatedSupports)
                        {
                            b.AddSupports(gameObject, i);
                            supporting[i] = hits[i].collider.gameObject;
                        }

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
        supported = true;

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
                    if (!b.populatedSupports)
                    {
                        b.AddSupports(gameObject, i);
                        supporting[i] = hits[i].collider.gameObject;
                    }

                }
            }
        }

        //set this variable to true so the object knows it doesn't need to update other nearby objects again
        populatedSupports = true;

    }


    //called by objects supporting this object when they break to remove themselves from the supports array and check to see if the object 
    //is still supported
    void UpdateSupports(GameObject g)
    {
        //remove the destroyed object that called this method from the supports list
        for(int i = 0; i<4; i++)
        {
            Debug.Log(g);

            if (supports[i] == g)
            {
                
                supports[i] = null;
            }
        }

        //check if there are any objects still supporting this one
        int remainingConnections = 0;
        foreach(GameObject game in supports)
        {
            if(game != null)
            {
                remainingConnections++;
            }
        }

        //if there are no longer any objects supporting this one destroy it after a delay
        if(remainingConnections == 0)
        {

            StartCoroutine(ExecuteAftertime(deathTime));
        }

        
    }


    private void AlertConnected()
    {
        //when this object is destroyed (can replace with another function later if I want) update nearby objects by removing this one as a support
        for(int i = 0; i < 4; i++)
        {
            //Debug.Log(supporting[i]);
            if (supporting[i] != null) {

                if (supporting[i].TryGetComponent(out BreakableStructureScript b))
                {

                    b.UpdateSupports(gameObject);


                }
            }
        }
        StartCoroutine(DestroyAftertime(deathTime));
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0 && !dead)
        {
            StartCoroutine(ExecuteAftertime(deathTime));
            dead = true;
        }
    }


    IEnumerator ExecuteAftertime(float time)
    {
        yield return new WaitForSeconds(time/2);

        AlertConnected();
    }

    IEnumerator DestroyAftertime(float time)
    {
        yield return new WaitForSeconds(time / 2);

        Destroy(gameObject);
    }
}
