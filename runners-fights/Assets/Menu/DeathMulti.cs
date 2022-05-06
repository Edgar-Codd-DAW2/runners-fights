using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class DeathMulti : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    //private Text roomName;

    /*void Start()
    {
        roomName.text = "Sala " + PhotonNetwork.CurrentRoom.Name;
    }*/

    
    public void Respawn()
    {
        //this.gameObject.GetComponent<PlayerMovementMuli>().Respawn();
        Transform[] spawnPoints = GameObject.FindWithTag("PlayerSpawner").transform.GetComponentsInChildren<Transform>();

        //int randomNumber = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPoint = spawnPoints[0].position;

        player.GetComponent<PhotonView>().RPC("Respawn", RpcTarget.AllBuffered, spawnPoint);
        //GameObject.Find("MainCamera").GetComponent<Camera>().enabled = true;
    }

    public void Leave()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
        //PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer.ActorNumber);
        //PhotonNetwork.Disconnect();
        Debug.Log("Leaving");
        //SceneManager.LoadScene("Menu");
        //StartCoroutine(DisconnectAndLeave());
        GameObject.FindWithTag("PlayerSpawner").GetComponent<PlayerSpawner>().DisconnectPlayer();
    }

    /*public override void OnConnectedToMaster()
    {
        Debug.Log("Conected");
        PhotonNetwork.LoadLevel("Lobby");
    }*/

    /*public override void OnLeftRoom()
    {
        Debug.Log("Left");
        PhotonNetwork.LoadLevel("Lobby");
    }*/

    /*private IEnumerator DisconnectAndLeave()
    {
        Debug.Log("Co");
        PhotonNetwork.Disconnect();
        Debug.Log("routine");
        yield return new WaitForSeconds(3f);
        Debug.Log("Scene");
        SceneManager.LoadScene("Menu");
    }*/
}
