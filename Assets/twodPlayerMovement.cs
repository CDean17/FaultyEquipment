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

    
    // Start is called before the first frame update
    void Start()
    {
        rb2d = transform.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if can jump or not
        canJump = Physics2D.Raycast(transform.position, -transform.up, 1.2f);

        moveDirection = Input.GetAxis("Horizontal");

        if (Input.GetAxis("Vertical") > 0 && canJump)
            jumpNow = true;

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
        }

    }

    private void jump(float power)
    {

    }
}
