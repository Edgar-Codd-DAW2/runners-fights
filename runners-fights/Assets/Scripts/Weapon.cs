using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum WeaponState
{
    idle,
    attack,
}

public class Weapon : MonoBehaviour
{
    // Start is called before the first frame update
    public WeaponState currentState;
    private bool pickUpAllowed;
    public GameObject player;
    private BoxCollider2D boxCollider2D;
    public Animator animator;
    public bool isMelee;
    public GameObject bulletPreFab;
    public Transform bulletPosicion;


    // Use this for initialization
    private void Start()
    {
        currentState = WeaponState.idle;

        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (pickUpAllowed && Input.GetKeyDown(KeyCode.E) && player != null)
        {
            PickUp();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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
                    player1.Hit();
                }
            }

            if (turrets != null)
            {
                turrets.Hit();
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && transform.parent == null)
        {
            player = null;
            pickUpAllowed = false;
        }
    }

    public bool IsMelee()
    {
        return isMelee;
    }

    public void Attack()
    {
        if (isMelee)
        {
            StartCoroutine(AttackCo());
        } 
        else
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector3 direction;
        if (player.transform.localScale.x == 1.0f) direction = Vector2.right;
        else direction = Vector2.left;

        GameObject bullet = Instantiate(bulletPreFab, bulletPosicion.transform.position + direction * 0.5f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);
    }

    private void PickUp()
    {

        player.GetComponent<PlayerMovement>().PickUP(gameObject);
        transform.SetParent(player.transform.GetChild(1).transform);

        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        
        boxCollider2D.enabled = false;

        player = transform.parent.parent.gameObject;
        //Destroy(gameObject);
    }

    public void Drop()
    {
        boxCollider2D.enabled = true;
        player = null;
        transform.parent = null;
        //Destroy(gameObject);
    }

    private IEnumerator AttackCo()
    {
        animator.SetTrigger("attack");
        currentState = WeaponState.attack;
        yield return null;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        currentState = WeaponState.idle;
    }
}
