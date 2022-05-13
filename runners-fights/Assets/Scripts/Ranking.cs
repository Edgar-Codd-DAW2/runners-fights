using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Networking;

public class Ranking : MonoBehaviour
{
    [SerializeField] string email;

    [SerializeField] Text errorMessages;

    WWWForm form;

    public void SendToRanking(string playerEmail)
    {
        email = playerEmail;

        Debug.Log("Update " + playerEmail + " ranking's");
        //StartCoroutine(UpdateRankingCo());
    }

    IEnumerator UpdateRankingCo()
    {
        form = new WWWForm();

        form.AddField("email", email);

        UnityWebRequest www = UnityWebRequest.Put("http://127.0.0.1:8080/api/kills", "{'email' :'" + email + "'}");

        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
        }
    }
}
