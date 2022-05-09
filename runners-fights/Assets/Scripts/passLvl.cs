using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class passLvl : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerPrefs.SetInt(PhotonNetwork.NickName, 2);
            SceneManager.LoadScene(6);
        }
    }
}
