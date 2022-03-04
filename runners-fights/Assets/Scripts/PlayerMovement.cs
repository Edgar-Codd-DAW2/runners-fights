using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public enum PlayerState
{
    walk,
    attack,
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
    private int Health = 5;
    public Animator animator;
    public bool grounded;
    public float speed;
    public Transform checkGround;
    public Transform arm;
    public Vector3 checkBoxSize;
    public float attakRange;
    public LayerMask platformLayerMask;
    public LayerMask enemyLayerMask;
    public bool isMelee;
    public float attackRate;
    public AudioClip hurtSound;
    public Text healthText;
    public GameObject gameOverUI;

    private SpriteRenderer myRenderer;
    private Shader shaderGUItext;
    private Shader shaderSpritesDefault;

    PhotonView view;
   
    public bool usingLadder = false;

    void Start()
    {
        currentState = PlayerState.walk;
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        myRenderer = gameObject.GetComponent<SpriteRenderer>();
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");

        view = GetComponent<PhotonView>();
    }
    //capturar input de teclado valores de 1 a -1
    void Update()
    {
        if (view.IsMine)
        {
            healthText.text = Health.ToString();
            if (currentState != PlayerState.attack)
            {
                horizontal = Input.GetAxisRaw("Horizontal");

                if (horizontal < 0.0f)
                {
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                    transform.GetChild(2).transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                }
                else if (horizontal > 0.0f)
                {
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    transform.GetChild(2).transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }

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


            if (Input.GetKeyDown(KeyCode.W) && grounded && currentState != PlayerState.attack)
            {
                Jump();
            }

            if (Input.GetKeyDown(KeyCode.J) && Time.time > lastShot + attackRate && currentState != PlayerState.defend)
            {
                if (arm.GetComponent<Equip>().IsWeaponSet())
                {
                    currentState = PlayerState.attack;
                    arm.GetComponent<Equip>().Attack(gameObject);
                    currentState = PlayerState.walk;
                }
                else
                {
                    if (isMelee)
                    {
                        StartCoroutine(AttackCo());
                    }
                    else
                    {
                        Shoot();
                    }
                    lastShot = Time.time;
                }
            }

            if (Input.GetKeyDown(KeyCode.K) && currentState != PlayerState.attack)
            {
                StartCoroutine(DefendCo());
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(checkGround.position, checkBoxSize);
        Gizmos.DrawWireSphere(arm.position, attakRange);
    }

        private void Jump() 
    {
        rigidbody2D.AddForce(Vector2.up * jumpForce);


        //animator.SetFloat("speed", rigidbody2D.velocity.y);
    }
    /*private void Attack()
    {
        animator.SetTrigger("attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(arm.position, attakRange, enemyLayerMask);
    
        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit");
        }
    }*/

    public void PickUP(GameObject weapon)
    {
        //GameObject cpWeapon = Instantiate(weapon, arm.position, Quaternion.identity);
        arm.GetComponent<Equip>().SetWeapon(weapon);
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

    public void Hit()
    {
        if (currentState != PlayerState.defend)
        {
            //Camera.main.GetComponent<AudioSource>().PlayOneShot(hurtSound);
            Health = Health - 1;

            if (Health <= 0)
            {
                GetComponent<Renderer>().enabled = false;
                gameOverUI.SetActive(true);
                Time.timeScale = 0f;
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player1 = collision.GetComponent<PlayerMovement>();
        TurretScript turrets = collision.GetComponent<TurretScript>();
        if (player1 != null)
        {
            player1.Hit();
        }

        if (turrets != null)
        {
            turrets.Hit();
        }
    }

    private IEnumerator AttackCo()
    {
        animator.SetTrigger("attack");
        currentState = PlayerState.attack;
        yield return null;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        currentState = PlayerState.walk;
    }
}
