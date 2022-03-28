using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Health : MonoBehaviour
{
    // Start is called before the first frame update
    public Image healthBar;
    PhotonView view;

    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    public void reduceHealth(float amount)
    {
        if(view.IsMine)
        {
            healthBar.fillAmount -= amount;
        } else
        {
            healthBar.fillAmount -= amount;
        }
    }
}
