using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletScriptMulti : BulletScript
{
    private string owner;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("0");
        if (!GetComponent<PhotonView>().IsMine) return;
        Debug.Log("1");
        PhotonView playerView = collision.gameObject.GetComponent<PhotonView>();
        Debug.Log("2");
        if (playerView != null)
        {
            Debug.Log("3");
            playerView.RPC("Hit", RpcTarget.AllBuffered, damage, owner);
            Debug.Log("4");
        }

        if (collision.gameObject.layer != LayerMask.NameToLayer("Item") && !collision.gameObject.CompareTag("Ladder"))
        {
            GetComponent<PhotonView>().RPC("DestroyBullet", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void SetOwner(string newOwner)
    {
        Debug.Log(owner);
        owner = newOwner;
    }
}
