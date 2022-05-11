using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using UnityEngine.Networking;


public class passLvl3 : MonoBehaviour
{
    public WWWForm form;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*TODO
         * StartCoroutine(SetLevel3Co(PhotonNetwork.NickName)); 
         */
        if (collision.gameObject.tag == "Player")
        {
            PlayerPrefs.SetInt(PhotonNetwork.NickName, 3);
            SceneManager.LoadScene("Mapa3");
        }
    }

    IEnumerator SetLevel3Co(string email)
    {
        form = new WWWForm();

        form.AddField("email", email);
        form.AddField("level", 3);
        UnityWebRequest www = UnityWebRequest.Post(/*TODO API URL*/"", form);

        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Level updated");
        }
    }
}
