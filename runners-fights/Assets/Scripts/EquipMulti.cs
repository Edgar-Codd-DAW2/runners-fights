using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EquipMulti : Equip
{
    public PhotonView view;
    // Start is called before the first frame update
   void Start()
    {
        view = gameObject.GetComponent<PhotonView>();
    }
    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            if (weapon != null)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    weapon.GetComponent<PhotonView>().RPC("Drop", RpcTarget.AllBuffered);
                    weapon = null;
                }
            }
        }
    }

    [PunRPC]
    public void SetWeaponMulti(int viewID)
    {
        weapon = PhotonView.Find(viewID).gameObject;
        weapon.transform.position = transform.position;
        weapon.GetComponent<PhotonView>().RPC("SetOwner", RpcTarget.AllBuffered, PhotonNetwork.NickName);
    }
}
