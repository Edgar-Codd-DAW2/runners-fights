using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public void OnClickConnect(string user)
    {
        PhotonNetwork.NickName = user;
        PhotonNetwork.AutomaticallySyncScene = true;
        
        if (user == "admin@admin.com")
        {
            PlayerPrefs.SetInt(user, 4);
        } else if (!PlayerPrefs.HasKey(user))
        {
            PlayerPrefs.SetInt(user, 1);
        }
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Menu");
    }
}
