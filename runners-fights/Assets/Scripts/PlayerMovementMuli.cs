using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerMovementMuli : PlayerMovement
{
    private string lastPlayerToHit;
    public PhotonView view;
   
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
        if (view.IsMine)
        {
            currentState = PlayerState.walk;
            rigidbody2D = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            myRenderer = gameObject.GetComponent<SpriteRenderer>();
            shaderGUItext = Shader.Find("GUI/Text Shader");
            shaderSpritesDefault = Shader.Find("Sprites/Default");
        }
    }
    //capturar input de teclado valores de 1 a -1
    void Update()
    {
        if (view.IsMine && gameObject.GetComponent<CapsuleCollider2D>().enabled && healthBar.fillAmount > 0)
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
                if (arm.GetComponent<Equip>().weapon != null)
                {
                    currentState = PlayerState.attack;
                    arm.GetComponent<PhotonView>().RPC("Attack", RpcTarget.AllBuffered);
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
        else if (view.IsMine)
        {
            gameOverUI.SetActive(true);
        } else
        {
            gameOverUI.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        if (view.IsMine)
        {
            rigidbody2D.velocity = new Vector2(horizontal * speed, rigidbody2D.velocity.y);
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

    [PunRPC]
    public void PickUP(int viewID)
    {
        //GameObject cpWeapon = Instantiate(weapon, arm.position, Quaternion.identity);
        arm.GetComponent<PhotonView>().RPC("SetWeaponMulti", RpcTarget.AllBuffered, viewID);
    }

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
        bullet.GetComponent<PhotonView>().RPC("SetOwner", RpcTarget.AllBuffered, PhotonNetwork.NickName);
    }


    protected override IEnumerator DefendCo()
    {
        view.RPC("SetPlayerState", RpcTarget.AllBuffered, (byte)PlayerState.defend);
        yield return null;
        view.RPC("SetWhite", RpcTarget.AllBuffered);
        yield return new WaitForSeconds(3f);
        view.RPC("SetNormal", RpcTarget.AllBuffered);
        view.RPC("SetPlayerState", RpcTarget.AllBuffered, (byte)PlayerState.walk);
    }

    [PunRPC]
    private void SetPlayerState(byte playerState)
    {
        currentState = (PlayerState)playerState;
    }

    [PunRPC]
    private void SetWhite()
    {
        myRenderer.material.shader = shaderGUItext;
        myRenderer.color = Color.white;
    }

    [PunRPC]
    private void SetNormal()
    {
        myRenderer.material.shader = shaderSpritesDefault;
        myRenderer.color = Color.white;
    }

    [PunRPC]
    public void Hit(float amount, string name)
    {
        if (currentState != PlayerState.defend)
        {
            healthBar.fillAmount -= amount / health / 10;
            if (name != "")
            {
                Debug.Log("Hit by: " + name);
                lastPlayerToHit = name;
            }

            if (healthBar.fillAmount <= 0)
            {
                view.RPC("Die", RpcTarget.AllBuffered);
                SendToRanking();
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        PhotonView playerView = collision.gameObject.GetComponent<PhotonView>();
        if (playerView != null && collision.gameObject.CompareTag("Player"))
        {
            playerView.RPC("Hit", RpcTarget.AllBuffered, damage, PhotonNetwork.NickName);
        }
    }

    [PunRPC]
    private void Die()
    {
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        transform.GetChild(2).gameObject.SetActive(false);
        GetComponent<Renderer>().enabled = false;
        gameOverUI.SetActive(true);
    }

    public void SendToRanking()
    {
        Debug.Log("Killed by: " + lastPlayerToHit);
    }

    [PunRPC]
    protected override void playerPosition()
    {
        transform.GetChild(2).transform.localScale = new Vector3(transform.localScale.x, 1, 1);
    }

    [PunRPC]
    public void Respawn(Vector3 respwanPosition)
    {
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        gameOverUI.SetActive(false);
        transform.position = respwanPosition;
        transform.GetChild(2).gameObject.SetActive(true);
        healthBar.fillAmount = 1f;
        GetComponent<Renderer>().enabled = true;
    }
}
