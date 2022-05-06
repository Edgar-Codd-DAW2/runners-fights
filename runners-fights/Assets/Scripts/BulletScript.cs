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
    public Rigidbody2D rigidbody2D;
    public Vector3 direction;
    public Camera camera;
    public AudioClip hurt;

    void Start()
    {
        camera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        camera.GetComponent<AudioSource>().PlayOneShot(hurt);


    }
        
    private void FixedUpdate()
    {
        if (direction == Vector3.left) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
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
    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    [PunRPC]
    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    [PunRPC]
    public void SetDamage(float amount)
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
            camera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
            camera.GetComponent<AudioSource>().PlayOneShot(hurt);
        }

        if (turrets != null)
        {
            turrets.Hit(damage);
            camera = GameObject.FindWithTag("PlayerCamera").GetComponent<Camera>();
            camera.GetComponent<AudioSource>().PlayOneShot(hurt);
        }
        if (collision.gameObject.layer != LayerMask.NameToLayer("Item") && !collision.gameObject.CompareTag("Ladder"))
        {
            DestroyBullet();
        }
    }
}
