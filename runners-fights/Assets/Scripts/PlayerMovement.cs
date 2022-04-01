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
    public float damage;
    public AudioClip hurtSound;
    public float health;
    public Image healthBar;
    public Text playerName;
    public GameObject gameOverUI;
    public GameObject playerCamera;

    private SpriteRenderer myRenderer;
    private Shader shaderGUItext;
    private Shader shaderSpritesDefault;

    PhotonView view;
   
    public bool usingLadder = false;

    void Awake()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            playerCamera.SetActive(true);
            playerName.text = PhotonNetwork.NickName;
        } else
        {
            playerName.text = view.Owner.NickName;
        }
    }

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
        if (view.IsMine)
        {
            if (PhotonNetwork.InRoom)
            {
                //view.RPC("playerHealthText", RpcTarget.AllBuffered);
            } else
            {
                //playerHealthText();
            }
            if (currentState != PlayerState.attack)
            {
                horizontal = Input.GetAxisRaw("Horizontal");

                if (horizontal < 0.0f)
                {
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                }
                else if (horizontal > 0.0f)
                {
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }

                if (PhotonNetwork.InRoom)
                {
                    view.RPC("playerPosition", RpcTarget.AllBuffered);
                } else
                {
                    playerPosition();
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

        /*GameObject bullet = Instantiate(bulletPreFab, transform.position + direction * 0.5f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);*/
        GameObject bullet = null;

        if (PhotonNetwork.CurrentRoom != null)
        {
            bullet = PhotonNetwork.Instantiate(bulletPreFab.name, transform.position + direction * 0.5f, Quaternion.identity);
        } else
        {
            bullet = Instantiate(bulletPreFab, transform.position + direction * 0.5f, Quaternion.identity);
        }
        bullet.GetComponent<BulletScript>().SetDirection(direction);
        bullet.GetComponent<BulletScript>().SetDamage(damage);
        //bullet.GetComponent<PhotonView>().RPC("SetDirection", RpcTarget.AllBuffered);
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

    [PunRPC]
    public void Hit(float amount)
    {
        if (currentState != PlayerState.defend)
        {
            healthBar.fillAmount -= amount / health / 10;

            if (healthBar.fillAmount <= 0)
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
            if (PhotonNetwork.InRoom)
            {
                view.RPC("Hit", RpcTarget.AllBuffered, damage);
            }
            else
            {
                player1.Hit(damage);
            }
        }

        if (turrets != null)
        {
            turrets.Hit(damage);
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

    [PunRPC]
    private void playerPosition()
    {

        transform.GetChild(2).transform.localScale = new Vector3(transform.localScale.x, 1, 1);
    }

    [PunRPC]
    public void reduceHealth(float amount)
    {
        if (view.IsMine)
        {
            healthBar.fillAmount -= amount;
        }
        else
        {
            healthBar.fillAmount -= amount;
        }
    }
}
