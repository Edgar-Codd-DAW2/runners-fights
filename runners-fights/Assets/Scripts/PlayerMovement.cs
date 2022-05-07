using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public enum PlayerState : byte
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

    protected Rigidbody2D rigidbody2D;
    protected float horizontal;
    protected float lastShot;
    protected int Health;
    public Animator animator;
    public bool grounded;
    public float speed;
    public Transform checkGround;
    public Transform arm;
    public Vector3 checkBoxSize;
    public LayerMask platformLayerMask;
    public bool isMelee;
    public float attackRate;
    public float damage;
    public AudioClip hurtSound;
    public float health;
    public Image healthBar;
    public Text playerName;
    public GameObject gameOverUI;
    public GameObject playerCamera;

    public SpriteRenderer myRenderer;
    public Shader shaderGUItext;
    public Shader shaderSpritesDefault;

   
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

            
            playerPosition();

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
                arm.GetComponent<Equip>().Attack();
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

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(checkGround.position, checkBoxSize);
    }

    protected void Jump() 
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

    protected virtual void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x == 1.0f) direction = Vector2.right;
        else direction = Vector2.left;

        /*GameObject bullet = Instantiate(bulletPreFab, transform.position + direction * 0.5f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);*/

        GameObject bullet = Instantiate(bulletPreFab, arm.position + direction * 0.5f, Quaternion.identity);

        bullet.GetComponent<BulletScript>().SetDirection(direction);
        bullet.GetComponent<BulletScript>().SetDamage(damage);
    }

    void FixedUpdate()
    {
        rigidbody2D.velocity = new Vector2(horizontal * speed, rigidbody2D.velocity.y);
    }

    protected virtual IEnumerator DefendCo()
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

    /*
    protected void whiteSprite()
    {
        myRenderer.material.shader = shaderGUItext;
        myRenderer.color = Color.white;
    }*/

    /*protected void normalSprite()
    {
        myRenderer.material.shader = shaderSpritesDefault;
        myRenderer.color = Color.white;
    }*/


    public virtual void Hit(float amount)
    {
        if (currentState != PlayerState.defend)
        {
            healthBar.fillAmount -= amount / health / 10;

            if (healthBar.fillAmount <= 0)
            {
                transform.GetChild(2).gameObject.SetActive(false);
                GetComponent<Renderer>().enabled = false;
                gameOverUI.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    public void Death()
    {
        healthBar.fillAmount = 0;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        TurretScript turrets = collision.GetComponent<TurretScript>();
        if (player != null)
        {
            player.Hit(damage);
        }

        if (turrets != null)
        {
            turrets.Hit(damage);
        }
    }

    protected IEnumerator AttackCo()
    {
        animator.SetTrigger("attack");
        currentState = PlayerState.attack;
        yield return null;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        currentState = PlayerState.walk;
    }


    protected virtual void playerPosition()
    {
        transform.GetChild(2).transform.localScale = new Vector3(transform.localScale.x, 1, 1);
    }
}
