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


            PlayerPosition();

            animator.SetBool("running", horizontal != 0.0f);
        }


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
    }

    public void PickUP(GameObject weapon)
    {
        arm.GetComponent<Equip>().SetWeapon(weapon);
    }

    protected virtual void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x == 1.0f) direction = Vector2.right;
        else direction = Vector2.left;

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
        TurretScript turrets = collision.GetComponent<TurretScript>();

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


    protected virtual void PlayerPosition()
    {
        transform.GetChild(2).transform.localScale = new Vector3(transform.localScale.x, 1, 1);
    }
}
