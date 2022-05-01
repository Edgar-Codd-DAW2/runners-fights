using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletScriptMulti : BulletScript
{
    private string ownerName;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (!GetComponent<PhotonView>().IsMine) return;

        PhotonView playerView = collision.gameObject.GetComponent<PhotonView>();

        if (playerView != null)
        {
            playerView.RPC("Hit", RpcTarget.AllBuffered, damage, ownerName);
        }

        if (collision.gameObject.layer != LayerMask.NameToLayer("Item") && !collision.gameObject.CompareTag("Ladder"))
        {
            GetComponent<PhotonView>().RPC("DestroyBullet", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void SetOwner(string owner)
    {
        Debug.Log(owner);
        ownerName = owner;
    }
}
