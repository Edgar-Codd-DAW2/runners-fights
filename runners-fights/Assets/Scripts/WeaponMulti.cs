using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponMulti : Weapon
{
    public PhotonView view;
    public string owner;
    void Start()
    {
        view = GetComponent<PhotonView>();

            currentState = WeaponState.idle;

            boxCollider2D = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
            timeToDestroy = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            /*if (pickUpAllowed && Input.GetKeyDown(KeyCode.E) && player != null)
            {
                view.RPC("PickUp", RpcTarget.AllBuffered);
            }*/

            if (transform.parent == null)
            {
                /*if (timeToDestroy > 0)
                {
                    timeToDestroy -= Time.deltaTime;
                }
                else
                {
                    Destroy(gameObject);
                }*/
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("!!!!!!!!!!");
        if (currentState == WeaponState.attack)
        {
            PhotonView playerView = collision.gameObject.GetComponent<PhotonView>();
            if (playerView != null && playerView.gameObject.CompareTag("Player"))
            {
                playerView.RPC("Hit", RpcTarget.AllBuffered, damage, owner);
            }
        }
        else
        {
            if (collision.gameObject.CompareTag("Player") && collision.transform.GetChild(1).GetComponent<Equip>().weapon == null)
            {
                PhotonView playerView = collision.gameObject.GetComponent<PhotonView>();
                Debug.Log("!?!?!?!?!?");
                view.RPC("SetPlayerOwner", RpcTarget.AllBuffered, playerView.ViewID);
            }
        }
       
    }

    [PunRPC]
    public void SetPlayerOwner(int playerViewId)
    {
        player = PhotonView.Find(playerViewId).gameObject;
        pickUpAllowed = true;
    }

        [PunRPC]
    public void SetOwner(string newOwner)
    {
        Debug.Log(owner);
        owner = newOwner;
    }

    [PunRPC]
    public override void Attack()
    {
        if (isMelee)
        {
            StartCoroutine(AttackCo());
        }
        else
        {
            view.RPC("Shoot", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    protected override void Shoot()
    {
        Vector3 direction;
        if (player.transform.localScale.x == 1.0f) direction = Vector2.right;
        else direction = Vector2.left;

        GameObject bullet = PhotonNetwork.Instantiate(bulletPreFab.name, bulletPosicion.transform.position + direction * 0.5f, Quaternion.identity);
        bullet.GetComponent<PhotonView>().RPC("SetDirection", RpcTarget.AllBuffered, direction);
        bullet.GetComponent<PhotonView>().RPC("SetDamage", RpcTarget.AllBuffered, damage);
        bullet.GetComponent<PhotonView>().RPC("SetOwner", RpcTarget.AllBuffered, owner);

    }


    [PunRPC]
    protected void PickUp(int playerViewId)
    {
        if (pickUpAllowed && player != null && GameObject.ReferenceEquals(player, PhotonView.Find(playerViewId).gameObject))
        {
            player.GetComponent<PhotonView>().RPC("PickUP", RpcTarget.AllBuffered, view.ViewID);
            transform.SetParent(player.transform.GetChild(1).transform);

            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            boxCollider2D.enabled = false;

            player = transform.parent.parent.gameObject;
        }
    }

    protected override IEnumerator AttackCo()
    {
        animator.SetTrigger("attack");
        view.RPC("SetWeaponState", RpcTarget.AllBuffered, (byte)WeaponState.attack);
        yield return null;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        view.RPC("SetWeaponState", RpcTarget.AllBuffered, (byte)WeaponState.idle);
    }

    [PunRPC]
    private void SetWeaponState(byte weaponState)
    {
        currentState = (WeaponState)weaponState;
    }
}
