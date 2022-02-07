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
    private GameObject player;
    private BoxCollider2D boxCollider2D;
    public Animator animator;


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
        if (pickUpAllowed && Input.GetKeyDown(KeyCode.E))
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
            Debug.Log(collision);
            if (player1 != null)
            {
                player1.Hit();
            }

            if (turrets != null)
            {
                turrets.Hit();
            }
        }
        else 
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                player = collision.gameObject;
                pickUpAllowed = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player = null;
            pickUpAllowed = false;
        }
    }

    public void Attack()
    {
        StartCoroutine(AttackCo());
    }

    private void PickUp()
    {
        player.GetComponent<PlayerMovement>().PickUP(gameObject);
        transform.SetParent(player.transform.GetChild(1).transform);
        boxCollider2D.enabled = false;
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
