using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerMovementMuli : PlayerMovement
{
    PhotonView view;
   
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

                
                 view.RPC("playerPosition", RpcTarget.AllBuffered);

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

    /*private void Attack()
    {
        animator.SetTrigger("attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(arm.position, attakRange, enemyLayerMask);
    
        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit");
        }
    }*/

    /*public void PickUP(GameObject weapon)
    {
        //GameObject cpWeapon = Instantiate(weapon, arm.position, Quaternion.identity);
        arm.GetComponent<Equip>().SetWeapon(weapon);
    }*/

    protected override void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x == 1.0f) direction = Vector2.right;
        else direction = Vector2.left;

        /*GameObject bullet = Instantiate(bulletPreFab, transform.position + direction * 0.5f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);*/

        GameObject bullet = PhotonNetwork.Instantiate(bulletPreFab.name, arm.position + direction * 0.5f, Quaternion.identity);
        bullet.GetComponent<PhotonView>().RPC("SetDirection", RpcTarget.AllBuffered, direction);
        bullet.GetComponent<PhotonView>().RPC("SetDamage", RpcTarget.AllBuffered, damage);
    }


    /*protected override IEnumerator DefendCo()
    {
        currentState = PlayerState.defend;
        yield return null;
        myRenderer.material.shader = shaderGUItext;
        myRenderer.color = Color.white;
        yield return new WaitForSeconds(3f);
        myRenderer.material.shader = shaderSpritesDefault;
        myRenderer.color = Color.white;
        currentState = PlayerState.walk;
    }*/

    [PunRPC]
    public override void Hit(float amount)
    {
        if (currentState != PlayerState.defend)
        {
            healthBar.fillAmount -= amount / health / 10;

            if (healthBar.fillAmount <= 0)
            {
                GetComponent<Renderer>().enabled = false;
                //gameOverUI.SetActive(true);
                //Time.timeScale = 0f;
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        PhotonView playerView = collision.gameObject.GetComponent<PhotonView>();
        if (playerView != null && collision.gameObject.CompareTag("Player"))
        {
            playerView.RPC("Hit", RpcTarget.AllBuffered, damage);
        }
    }

    [PunRPC]
    protected override void playerPosition()
    {
        transform.GetChild(2).transform.localScale = new Vector3(transform.localScale.x, 1, 1);
    }
}