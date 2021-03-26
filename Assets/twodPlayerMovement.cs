using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class twodPlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb2d;

    public float jumpPower = 30f;
    public float groundMoveSpeed = 10f;
    public float airStrafeSpeed = 2f;
    public float maxStrafeSpeed = 10f;  //compare the x value of the current movespeed to the direction hit to determine whether to apply. That way the player still has control when moving fast
    public float moveDirection = 0;
    public bool canJump = true;
    private bool jumpNow = false;
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

        moveDirection = Input.GetAxis("Horizontal");

        if (Input.GetAxis("Vertical") > 0 && canJump && !jumpNow)
        {
            jumpNow = true;
            anim.SetBool("Jump", true);
            canJump = false;
            anim.SetBool("Grounded", false);
        }
            
        if(rb2d.velocity.x != 0)
        {
            anim.SetBool("Walking", true);
        }
        else
        {
            anim.SetBool("Walking", false);
        }

        if(Input.GetAxis("Horizontal") < 0)
        {
            spr.flipX = true;
        }
        else if(Input.GetAxis("Horizontal") > 0)
        {
            spr.flipX = false;
        }

    }

    private void FixedUpdate()
    {
        float yFix = rb2d.velocity.y;

        rb2d.velocity = new Vector2(moveDirection * groundMoveSpeed, yFix);

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
            canJump = true;
          anim.SetBool("Grounded", true);

        // }
    }

   
 
}
