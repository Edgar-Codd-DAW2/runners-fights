using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    public InputField roomInputField;
    public GameObject loadingPanel;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public Text roomName;
    public Button stadium1Button;
    public Button stadium2Button;
    public string map;

    public RoomItem roomItemPrefab;
    List<RoomItem> roomItemList = new List<RoomItem>();
    public Transform contentObject;

    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    public List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    public GameObject playButton;
    public Text stadium1ButtonText;
    public Text stadium2ButtonText;

    void Start()
    {
        if (PhotonNetwork.OfflineMode || !PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.ConnectUsingSettings();
            StartCoroutine(Reconnect());
        } else
        {
            PhotonNetwork.JoinLobby();
            loadingPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }

    // Update is called once per frame
    /*void Update()
    {
    }*/

    public void OnClickStadium1()
    {
        if (roomInputField.text.Length >= 1)
        {
            map = "Stadiummulti1";
            stadium1ButtonText.text = "Creando...";
            CreateRoom();
        }
    }

    public void OnClickStadium2()
    {
        if (roomInputField.text.Length >= 1)
        {
            map = "Stadiummulti2";
            stadium2ButtonText.text = "Creando...";
            CreateRoom();
        }
    }

    private void CreateRoom()
    {
        stadium1Button.interactable = false;
        stadium2Button.interactable = false;
        PhotonNetwork.CreateRoom(roomInputField.text, new RoomOptions() { MaxPlayers = 4, BroadcastPropsChangeToAll = true });
    }

    public void OnClickLeaveLobby()
    {
        SceneManager.LoadScene("Menu");
    }

    public override void OnJoinedRoom()
    {
        loadingPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomName.text = "Sala " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdatedRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
    }

    void UpdatedRoomList(List<RoomInfo> list)
    {
        foreach (RoomItem item in roomItemList)
        {
            Destroy(item.gameObject);
        }
        roomItemList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, contentObject);
            newRoom.SetRoomName(room.Name);
            roomItemList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        loadingPanel.SetActive(true);
        PhotonNetwork.JoinRoom(roomName);
    }
    public void OnClickLeaveRoom()
    {
        stadium1Button.interactable = true;
        stadium2Button.interactable = true;
        PhotonNetwork.LeaveRoom(roomName);
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
  
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.setPlayerInfo(player.Value);
            
            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }

            playerItemsList.Add(newPlayerItem);
        } 
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel(map);
    }

    IEnumerator Reconnect()
    {
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return null;
        }
        loadingPanel.SetActive(false);
    }
}
