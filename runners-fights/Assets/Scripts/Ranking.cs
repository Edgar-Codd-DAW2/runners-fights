using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class Ranking : MonoBehaviour
{
    [SerializeField] string email;

    [SerializeField] Text errorMessages;

    WWWForm form;

    public void SendToRanking(string playerEmail)
    {
        email = playerEmail;

        Debug.Log("Update " + playerEmail + " ranking's");
        StartCoroutine(UpdateRankingCo());
    }

    IEnumerator UpdateRankingCo()
    {
        form = new WWWForm();

        form.AddField("email", email);

        UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:8080/api/kills", form);
        
        yield return www.Send();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
                
            Debug.Log("+1!");
            Debug.Log(www.downloadHandler.text);
        }
        Debug.Log(www.responseCode);
    }
}
