using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ladderMovementMulti : ladderMovement
{
    private PhotonView view;

    // Update is called once per frame
    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    void Update()
    {
        if (view.IsMine)
        {
            vertical = Input.GetAxis("Vertical");

            if (isLadder && Mathf.Abs(vertical) > 0f)
            {
                isClimbing = true;
            }
        } 
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (isClimbing)
            {
                rb.gravityScale = 0f;
                rb.velocity = new Vector2(rb.velocity.x, vertical * speed);

            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (!view.IsMine) return;
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (!view.IsMine) return;
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
            rb.gravityScale = 4f;
        }
    }
}
