using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerItem : MonoBehaviourPunCallbacks
{
    public Text playerName;

    public Color highlightColor;
    public GameObject leftArrowButton;
    public GameObject rightArrowButton;

    ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
    public Image playeAvatar;
    public Sprite[] avatars;

    Player player;

    private void Awake()
    {
        playerProperties["playeAvatar"] = 0;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void setPlayerInfo(Player _player)
    {
        playerName.text = _player.NickName;
        player = _player;
        UpdatePlayerItem(player);
    }

    public void ApplyLocalChanges()
    {
        leftArrowButton.SetActive(true);
        rightArrowButton.SetActive(true);
    }

    public void onClickLeftArrow()
    {
        if ((int)playerProperties["playeAvatar"] == 0)
        {
            playerProperties["playeAvatar"] = avatars.Length - 1;
        } else
        {
            playerProperties["playeAvatar"] = (int)playerProperties["playeAvatar"] - 1;
        }

        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public void onClickRightArrow()
    {
        if ((int)playerProperties["playeAvatar"] == avatars.Length - 1)
        {
            playerProperties["playeAvatar"] = 0;
        }
        else
        {
            playerProperties["playeAvatar"] = (int)playerProperties["playeAvatar"] + 1;
        }

        PhotonNetwork.SetPlayerCustomProperties(playerProperties);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (player == targetPlayer)
        {
            UpdatePlayerItem(targetPlayer);
        }
    }

    void UpdatePlayerItem(Player player)
    {
        if (player.CustomProperties.ContainsKey("playeAvatar"))
        {
            playeAvatar.sprite = avatars[(int)player.CustomProperties["playeAvatar"]];
            playerProperties["playeAvatar"] = (int)player.CustomProperties["playeAvatar"];
        } else
        {
            playerProperties["playeAvatar"] = 0;
        }
    }
}
