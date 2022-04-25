using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingDeathScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            player.Death();
            player.Hit(0);
        }
    }
}
