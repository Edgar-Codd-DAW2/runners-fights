using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerMovementMuli : PlayerMovement
{
    private string lastPlayerToHit;
    public PhotonView view;
    public PhotonView weaponView;
    public AudioClip hurt;

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

                
                 view.RPC("PlayerPosition", RpcTarget.AllBuffered);

                animator.SetBool("running", horizontal != 0.0f);
            }

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
                        view.RPC("SlashSound", RpcTarget.AllBuffered);
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

            if (Input.GetKeyDown(KeyCode.E) && weaponView != null) {
                weaponView.RPC("PickUp", RpcTarget.AllBuffered, view.ViewID);
                weaponView.RPC("SetOwner", RpcTarget.AllBuffered, PhotonNetwork.NickName);
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

    [PunRPC]
    public void SlashSound()
    {
        GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>().GetComponent<AudioSource>().PlayOneShot(slash);
    }


    [PunRPC]
    public void PickUP(int viewID)
    {
        arm.GetComponent<PhotonView>().RPC("SetWeaponMulti", RpcTarget.AllBuffered, viewID);
    }

    protected override void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x == 1.0f) direction = Vector2.right;
        else direction = Vector2.left;


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
            GameObject.FindWithTag("PlayerCamera").GetComponent<AudioSource>().PlayOneShot(hurt);

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
        PhotonView collisionView = collision.gameObject.GetComponent<PhotonView>();
        if (collisionView != null && collision.gameObject.CompareTag("Player"))
        {
            collisionView.RPC("Hit", RpcTarget.AllBuffered, damage, PhotonNetwork.NickName);
        }
        if (view.IsMine && collisionView != null && collision.gameObject.CompareTag("Weapon") )
        {
            weaponView = collisionView;
        }
    }

    [PunRPC]
    private void Die()
    {
        Debug.Log("Died");
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        transform.GetChild(2).gameObject.SetActive(false);
        GetComponent<Renderer>().enabled = false;
    }

    public void SendToRanking()
    {
        Debug.Log("Killed by: " + lastPlayerToHit);
        if (!string.IsNullOrEmpty(lastPlayerToHit))
        {
            GameObject.FindWithTag("Ranking").gameObject.GetComponent<Ranking>().SendToRanking(lastPlayerToHit);
        }
    }

    [PunRPC]
    protected override void PlayerPosition()
    {
        transform.GetChild(2).transform.localScale = new Vector3(transform.localScale.x, 1, 1);
    }

    [PunRPC]
    public void Respawn()
    {
        lastPlayerToHit = "";
        gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        gameOverUI.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
        healthBar.fillAmount = 1f;
        GetComponent<Renderer>().enabled = true;
        Transform[] spawnPoints = GameObject.FindWithTag("Respawn").transform.GetComponentsInChildren<Transform>();
        int randomNumber = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPoint = spawnPoints[randomNumber].position;
        transform.position = spawnPoint;
    }
}
