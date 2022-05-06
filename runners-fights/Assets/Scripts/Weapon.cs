using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public enum WeaponState : byte
{
    idle,
    attack,
}

public class Weapon : MonoBehaviour
{
    // Start is called before the first frame update
    public WeaponState currentState;
    public bool pickUpAllowed;
    public GameObject player;
    public BoxCollider2D boxCollider2D;
    public Animator animator;
    public bool isMelee;
    public GameObject bulletPreFab;
    public Transform bulletPosicion;
    public float damage;
    public float timeToDestroy;
    public Camera camera;
    public AudioClip corte;
    public AudioClip hurt;

    // Use this for initialization
    void Start()
    {
        currentState = WeaponState.idle;

        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        timeToDestroy = 10f;
    }

    // Update is called once per frame
     void Update()
     {
        /*if (pickUpAllowed && Input.GetKeyDown(KeyCode.E) && player != null)
        {
            PickUp();
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

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState == WeaponState.attack) 
        {
            PlayerMovement player1 = collision.GetComponent<PlayerMovement>();
            TurretScript turrets = collision.GetComponent<TurretScript>();
            if (player1 != null)
            {
                if (collision.gameObject != player)
                {
                    Debug.Log(player);
                    player1.Hit(damage);
                }
            }

            if (turrets != null)
            {
                turrets.Hit(damage);
            }
        }
        else 
        {
            if (collision.gameObject.CompareTag("Player") && !collision.transform.GetChild(1).GetComponent<Equip>().IsWeaponSet())
            {
                player = collision.gameObject;
                pickUpAllowed = true;
            }
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && transform.parent == null)
        {
            player = null;
            pickUpAllowed = false;
        }
    }

    public virtual void Attack()
    {
        if (isMelee)
        {
            StartCoroutine(AttackCo());
            camera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
            camera.GetComponent<AudioSource>().PlayOneShot(corte);
        } 
        else
        {
            Shoot();
        }
    }

    protected virtual void Shoot()
    {
        Vector3 direction;
        if (player.transform.localScale.x == 1.0f) direction = Vector2.right;
        else direction = Vector2.left;

        GameObject bullet = Instantiate(bulletPreFab, bulletPosicion.transform.position + direction * 0.5f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);
        bullet.GetComponent<BulletScript>().SetDamage(damage);

    }

    protected virtual void PickUp()
    {

        player.GetComponent<PlayerMovement>().PickUP(gameObject);
        transform.SetParent(player.transform.GetChild(1).transform);

        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        
        boxCollider2D.enabled = false;

        player = transform.parent.parent.gameObject;
        //Destroy(gameObject);
    }

    [PunRPC]
    public void Drop()
    {
        boxCollider2D.enabled = true;
        player = null;
        transform.parent = null;
        timeToDestroy = 5f;
    }

    protected virtual IEnumerator AttackCo()
    {
        animator.SetTrigger("attack");
        currentState = WeaponState.attack;
        yield return null;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        currentState = WeaponState.idle;
    }
}
