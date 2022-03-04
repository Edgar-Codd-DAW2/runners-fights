    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public Rigidbody2D rb;
    float timeToDestroy = 5f;
    void Start()
    {
       
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine("fallDown");
        }
    }


   IEnumerator fallDown()
    {
        yield return new WaitForSeconds(1f);
        rb.isKinematic = false;
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
