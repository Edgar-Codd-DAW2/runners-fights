using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;



public class LevelManager1 : MonoBehaviour
{
    // Start is called before the first frame update
    public Button[] levelButtons;
    void Start()
    {
        Debug.Log(PlayerPrefs.GetInt(PhotonNetwork.NickName));
        for (int i = 0; i < PlayerPrefs.GetInt(PhotonNetwork.NickName); i++)
        {
            Debug.Log(i);
            Debug.Log(levelButtons[i]);
            levelButtons[i].interactable = true;
        }
    }
}
