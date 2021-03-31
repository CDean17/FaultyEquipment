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
    public float curSpeed = 10f;
    public bool canJump = true;
    private bool jumpNow = false;
    private bool aiming = false;

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

    // Update is called once per frame
    void Update()
    {
        
   
        if(rb2d.velocity.x != 0)
        {
            anim.SetBool("Walking", true);
        }
        else
        {
            anim.SetBool("Walking", false);
        }


    }

    private void FixedUpdate()
    {
        if (aiming)
        {
            curSpeed = aimMoveSpeed;
        }
        else
        {
            curSpeed = groundMoveSpeed;
        }

        float yFix = rb2d.velocity.y;
        if (canJump)
        {
            rb2d.velocity = new Vector2(moveDirection * curSpeed, yFix);
        }
        else
        {
            if(rb2d.velocity.x < maxStrafeSpeed && moveDirection > 0)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x + (airStrafeSpeed), yFix);
            }else if(rb2d.velocity.x > -maxStrafeSpeed && moveDirection < 0)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x - (airStrafeSpeed), yFix);
            }
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
        //
        //if (collision.)
        // {
        //Debug.Log("collided!");
        if (!jumpNow)
        {
            canJump = true;
        }
            
          anim.SetBool("Grounded", true);

        // }
    }

    public void OnMove(InputValue input)
    {
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
        if(canJump && !jumpNow)
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


   
 
}
