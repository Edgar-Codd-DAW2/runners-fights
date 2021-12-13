using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerState
{
    walk,
    defend
}


public class PlayerMovement : MonoBehaviour
{
    public GameObject bulletPreFab;
    public float jumpForce;
    //public float Speed;

    private Rigidbody2D rigidbody2D;
    public Animator animator;
    private float horizontal;
    public bool grounded;
    private float lastShot;
    public float speed;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    //capturar input de teclado valores de 1 a -1
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal < 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (horizontal > 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        animator.SetBool("running", horizontal != 0.0f);

        Debug.DrawRay(transform.position, Vector3.down * 2.2f, Color.red);
        if (Physics2D.Raycast(transform.position, Vector3.down, 2.2f)) 
        {
            grounded = true;
        } else 
        {
            grounded = false;
        }


        if (Input.GetKeyDown(KeyCode.W) && grounded)
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.J) && Time.time > lastShot + 0.25f)
        {
            Shoot();
            lastShot = Time.time;
        }

    }

    private void Jump() 
    {
        rigidbody2D.AddForce(Vector2.up * jumpForce);
    }

    private void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x == 1.0f) direction = Vector2.right;
        else direction = Vector2.left;

        GameObject bullet = Instantiate(bulletPreFab, transform.position + direction * 0.5f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = new Vector2(horizontal * speed, rigidbody2D.velocity.y);
    }
}
