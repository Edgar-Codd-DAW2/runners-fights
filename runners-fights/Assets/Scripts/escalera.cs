using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class escalera : MonoBehaviour
{

    public Rigidbody2D rb;
    Animator anim;
    PlayerMovement controller;

    public CompositeCollider2D ground;


    public bool onLadder = false;

    public float climbSpeed;
    public float exitHop = 3f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerMovement>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("escalera"))
        {
            if(Input.GetAxisRaw("Vertical") != 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, Input.GetAxisRaw("Vertical") * climbSpeed);
                rb.gravityScale = 0;
                onLadder = true;
                ground.enabled = false;
                controller.usingLadder = onLadder;
            }
            else if ( Input.GetAxisRaw("Vertical") == 0 && onLadder)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            anim.setBool("onLadder", onLadder);
            anim.SetFloat("speed", Mathf.Abs(Input.GetAxisRaw("Vertical")))
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("escalera") && onLadder)
        {
            rb.gravityScale = 1;
            onLadder = false;
            controller.usindLadder = onLadder;
            ground.enabled = true;

         
        }
    }

}
