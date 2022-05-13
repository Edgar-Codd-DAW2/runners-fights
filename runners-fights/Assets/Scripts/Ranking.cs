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

        /*//byte[] myData = System.Text.Encoding.UTF8.GetBytes(emai);
        //UnityWebRequest www = UnityWebRequest.Put("http://127.0.0.1:8080/api/kills", myData);

        Debug.Log("{\"email\":\"" + email + "\"}");

        //UnityWebRequest www = UnityWebRequest.Put("http://127.0.0.1:8080/api/kills", "{\"email\":\"" + email + "\"}");
        //www.SetRequestHeader("Content-Type", "application/json");

        string str = "{\"email\":\"" + email + "\"}";
        JObject json = JObject.Parse(str);
        //byte[] myData = System.Text.Encoding.UTF8.GetBytes("{\"email\":\"" + email + "\"}");
        byte[] myData = json.Properties().Select(p => (byte)p.Value).ToArray();*/

        byte[] myData = System.Text.Encoding.UTF8.GetBytes(email);
        using (UnityWebRequest www = UnityWebRequest.Put("http://127.0.0.1:8080/api/kills", myData))
        {
            yield return www.SendWebRequest();

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
}
