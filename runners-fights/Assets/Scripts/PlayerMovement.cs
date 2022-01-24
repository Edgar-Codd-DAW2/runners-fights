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
    public PlayerState currentState;
    public GameObject bulletPreFab;
    public float jumpForce;
    //public float Speed;

    private Rigidbody2D rigidbody2D;
    private float horizontal;
    private float lastShot;
    public Animator animator;
    public bool grounded;
    public float speed;
    public Transform checkGround;
    public Vector3 checkBoxSize;
    public LayerMask platformLayerMask;

    private SpriteRenderer myRenderer;
    private Shader shaderGUItext;
    private Shader shaderSpritesDefault;


   
    public bool usingLadder = false;

    void Start()
    {
        currentState = PlayerState.walk;
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");
    }
    //capturar input de teclado valores de 1 a -1
    void Update()
    {
        if (currentState != PlayerState.defend)
        {
            horizontal = Input.GetAxisRaw("Horizontal");

            if (horizontal < 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
            else if (horizontal > 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            animator.SetBool("running", horizontal != 0.0f);
        }

        /*Debug.DrawRay(transform.position, Vector3.down * 2.2f, Color.red);
        if (Physics2D.Raycast(transform.position, Vector3.down, 2.2f)) 
        {
            grounded = true;
        } else {
       
            grounded = false;
        }*/


        grounded = Physics2D.OverlapBox(checkGround.position, checkBoxSize, 0f, platformLayerMask);


        if (Input.GetKeyDown(KeyCode.W)  && grounded && currentState != PlayerState.defend)
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.J) && Time.time > lastShot + 0.25f && currentState != PlayerState.defend)
        {
            Shoot();
            lastShot = Time.time;
        }

        if (Input.GetKey(KeyCode.K) && !animator.GetBool("running"))
        {
            StartCoroutine(DefendCo());
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(checkGround.position, checkBoxSize);
    }

        private void Jump() 
    {
        rigidbody2D.AddForce(Vector2.up * jumpForce);


        //animator.SetFloat("speed", rigidbody2D.velocity.y);
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

    private IEnumerator DefendCo()
    {
        currentState = PlayerState.defend;
        yield return null;
        myRenderer.material.shader = shaderGUItext;
        myRenderer.color = Color.white;
        yield return new WaitForSeconds(3f);
        myRenderer.material.shader = shaderSpritesDefault;
        myRenderer.color = Color.white;
        currentState = PlayerState.walk;
    }

    private void whiteSprite()
    {
        myRenderer.material.shader = shaderGUItext;
        myRenderer.color = Color.white;
    }

    private void normalSprite()
    {
        myRenderer.material.shader = shaderSpritesDefault;
        myRenderer.color = Color.white;
    }
}
