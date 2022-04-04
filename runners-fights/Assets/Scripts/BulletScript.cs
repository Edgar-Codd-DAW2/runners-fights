using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletScript : MonoBehaviour
{
    public AudioClip sound;
    public float speed;
    public float damage;
    public float LastShoot;
    private Rigidbody2D rigidbody2D;
    private Vector2 direction;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
       // Camera.main.GetComponent<AudioSource>().PlayOneShot(sound);
       
    }
        
    private void FixedUpdate()
    {
        if (direction == Vector2.left) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        rigidbody2D.velocity = direction * speed;

        if (LastShoot > 0)
        {
            LastShoot -= Time.deltaTime;
        } else {
            DestroyBullet();
        }
    }

    [PunRPC]
    public virtual void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }

    [PunRPC]
    public virtual void DestroyBullet()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    public virtual void SetDamage(float amount)
    {
        damage = amount;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player1 = collision.GetComponent<PlayerMovement>();
        TurretScript turrets = collision.GetComponent<TurretScript>();

        if (player1 != null)
        {
            player1.Hit(damage);
        }

        if (turrets != null)
        {
            turrets.Hit(damage);
        }
        if (collision.gameObject.layer != LayerMask.NameToLayer("Item") && !collision.gameObject.CompareTag("Ladder"))
        {
            DestroyBullet();
        }
    }
}
