using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class twodPlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public GameObject jumpParticleSpawn;
    public GameObject jumpParticle;

    public float jumpPower = 30f;
    public float groundMoveSpeed = 10f;
    public float aimMoveSpeed = 4f;
    public float airStrafeSpeed = 2f;
    public float maxStrafeSpeed = 10f;  //compare the x value of the current movespeed to the direction hit to determine whether to apply. That way the player still has control when moving fast
    public float moveDirection = 0;
    private float upDown = 0;
    public float curSpeed = 10f;
    public bool canJump = true;
    private bool jumpNow = false;
    private bool aiming = false;
    public bool ragDolling = false;
    private bool rotateRight = true;
    private bool spinning = false;
    public float ragDollRotateSpeed = 5f;
    public float ragDollRestPeriod = 1f;
    private Vector3 launchDirection;
    private bool launchNow = false;
    private float launchForce;

    public Collider2D groundDetection;

    private Animator anim;
    private SpriteRenderer spr;

    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = transform.GetComponent<Rigidbody2D>();
        anim = transform.GetComponent<Animator>();
        spr = transform.GetComponent<SpriteRenderer>();
    }



    private void FixedUpdate()
    {

        //Set animation bools for walking
        if (rb2d.velocity.x != 0)
        {
            anim.SetBool("Walking", true);
        }
        else
        {
            anim.SetBool("Walking", false);
        }

        //Set movement speed based on if aiming
        if (aiming)
        {
            curSpeed = aimMoveSpeed;
        }
        else
        {
            curSpeed = groundMoveSpeed;
        }

        float yFix = rb2d.velocity.y;

        if (launchNow)
        {
            launchNow = false;
            // Debug.Log(Quaternion.AngleAxis(launchDirection.z, Vector3.forward) * Vector3.up * launchForce);
            rb2d.AddForce(Quaternion.AngleAxis(launchDirection.z, Vector3.forward) * Vector3.up * launchForce);
        }

        //Determine if ragdolling/have controll of character
        if (ragDolling)
        {
            if (rotateRight && spinning)
            {
                transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0, 0, ragDollRotateSpeed * Time.deltaTime));
            }
            else if(!rotateRight && spinning)
            {
                transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + new Vector3(0, 0, -ragDollRotateSpeed * Time.deltaTime));
            }

        }
        else
        {
            //If the player is in control then set their movement velocity using these equations relevant to if they are in the air or not
            if (canJump)
            {
                rb2d.velocity = new Vector2(moveDirection * curSpeed, yFix);
            }
            else
            {
                if (rb2d.velocity.x < maxStrafeSpeed && moveDirection > 0)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x + (airStrafeSpeed), yFix);
                }
                else if (rb2d.velocity.x > -maxStrafeSpeed && moveDirection < 0)
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x - (airStrafeSpeed), yFix);
                }
            }
        }
        
        
        //Code for falling through platforms
        if(upDown< 0)
        {
            gameObject.layer = 9;
        }
        else
        {
            gameObject.layer = 7;
        }


        if (jumpNow)
        {
            //Debug.Log("Jumping!");
            rb2d.velocity = rb2d.velocity + new Vector2(0, jumpPower);
            jumpNow = false;
            anim.SetBool("Jump", false);
        }

    }


    private void OnTriggerStay2D(Collider2D collision)
    {

        if (!jumpNow)
        {
            canJump = true;
        }
            
        anim.SetBool("Grounded", true);

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ragDolling)
        {
            spinning = false;
            Debug.Log("Started getup");
            rb2d.velocity = new Vector2(0, 0);
            transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
            StartCoroutine(StopRagdollAfterTime(ragDollRestPeriod));
        }
    }



    public void OnMove(InputValue input)
    {
        upDown = input.Get<Vector2>().y;
        moveDirection = input.Get<Vector2>().x;
        if(moveDirection < 0)
        {
            spr.flipX = true;
        }
        else if(moveDirection > 0)
        {
            spr.flipX = false;
        }
        Debug.Log("InputActionTriggered!");
    }

    public void OnJump()
    {
        if(canJump && !jumpNow && !ragDolling)
        {
            jumpNow = true;
            anim.SetBool("Jump", true);
            canJump = false;
            anim.SetBool("Grounded", false);

            Instantiate(jumpParticle, jumpParticleSpawn.transform.position, jumpParticleSpawn.transform.rotation);
        }
    }

    public void OnAim(InputValue input)
    {
        if (input.Get<float>() == 0)
        {
            aiming = false;
        }
        else
        {
            aiming = true;
        }
    }

    public void OnAimCanceled()
    {
        aiming = false;
    }

    public void RagdollPlayer(Vector3 launchDirection, float throwForce)
    {
        launchForce = throwForce;
        this.launchDirection = launchDirection;
        launchNow = true;

        ragDolling = true;
        spinning = true;
        //Debug.Log("Threw Player! Direction: " + launchDirection +"  Force: " + throwForce);


        if (launchDirection.z > 180)
        {
            rotateRight = false;
        }
        else
        {
            rotateRight = true;
        }


    }

    IEnumerator StopRagdollAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        ragDolling = false;
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
   
 
}
