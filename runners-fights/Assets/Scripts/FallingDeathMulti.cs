using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FallingDeathMulti : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PhotonView player = collision.gameObject.GetComponent<PhotonView>();
            player.RPC("Hit", RpcTarget.AllBuffered, 1000.0f, "");
        }
    }
}
