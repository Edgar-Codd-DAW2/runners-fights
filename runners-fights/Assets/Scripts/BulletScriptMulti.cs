using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletScriptMulti : BulletScript
{   
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GetComponent<PhotonView>().IsMine) return;

        PhotonView playerView = collision.gameObject.GetComponent<PhotonView>();

        if (playerView != null)
        {
            playerView.RPC("Hit", RpcTarget.AllBuffered, damage);
        }

        if (collision.gameObject.layer != LayerMask.NameToLayer("Item") && !collision.gameObject.CompareTag("Ladder"))
        {
            GetComponent<PhotonView>().RPC("DestroyBullet", RpcTarget.AllBuffered);
        }
    }
}
