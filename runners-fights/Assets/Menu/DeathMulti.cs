using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class DeathMulti : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public GameObject player;
    private Text roomName;

    void Start()
    {
        roomName.text = "Sala " + PhotonNetwork.CurrentRoom.Name;
    }

    
    public void Respawn()
    {
        //this.gameObject.GetComponent<PlayerMovementMuli>().Respawn();
        Transform[] spawnPoints = GameObject.FindWithTag("PlayerSpawner").transform.GetComponentsInChildren<Transform>();

        int randomNumber = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPoint = spawnPoints[randomNumber].position;

        player.GetComponent<PhotonView>().RPC("Respawn", RpcTarget.AllBuffered, spawnPoint);
    }

    public void Leave()
    {
        //Debug.Log("Sala " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }
}
