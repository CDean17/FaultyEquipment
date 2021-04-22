using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public bool open = false;


    private Animator anim;
    private Collider2D col;
    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetComponent<Animator>();
        col = transform.GetComponent<Collider2D>();
    }


    public void ToggleOpen()
    {
        open = !open;
        col.enabled = !open;
        anim.SetBool("isOpen", open);
    }
}
