using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class OfflineMode : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    private void Start()
    {
        PhotonNetwork.Disconnect();
        StartCoroutine(Disconnect());
    }

    IEnumerator Disconnect()
    {
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        PhotonNetwork.OfflineMode = true;
    }
}
