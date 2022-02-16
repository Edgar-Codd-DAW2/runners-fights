using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoomItem : MonoBehaviour
{
    // Start is called before the first frame update
    public Text roomName;
    LobbyManager manager;
    void Start()
    {
        manager = FindObjectOfType<LobbyManager>();
    }

    // Update is called once per frame
    public void SetRoomName(string roomName)
    {
        this.roomName.text = roomName;
    }

    public void OnClickItem()
    {
        manager.JoinRoom(roomName.text);
    }
}
