using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Photon.Pun;



public class LevelManager1 : MonoBehaviour
{
    // Start is called before the first frame update
    public Button[] levelButtons;
    public WWWForm form;
    void Start()
    {
        /*TODO 
        StartCoroutine(GetLevelCo(PhotonNetwork.NickName)); 
         */

        //Temporal solution
        Debug.Log(PlayerPrefs.GetInt(PhotonNetwork.NickName));
        for (int i = 0; i < PlayerPrefs.GetInt(PhotonNetwork.NickName); i++)
        {
            Debug.Log(i);
            Debug.Log(levelButtons[i]);
            levelButtons[i].interactable = true;
        }
    }

    IEnumerator GetLevelCo(string email)
    {
        form = new WWWForm();

        form.AddField("email", email);
        UnityWebRequest www = UnityWebRequest.Post(/*TODO API URL*/"", form);

        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Level level = Level.CreateFromJSON(www.downloadHandler.text);
            level.GetLevel();

            for (int i = 0; i < level.GetLevel(); i++)
            {
                Debug.Log(i);
                Debug.Log(levelButtons[i]);
                levelButtons[i].interactable = true;
            }
        }
    }
}

public class Level
{
    // Start is called before the first frame update
    public int level;

    public int GetLevel()
    {
        return level;
    }

    public static Level CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Level>(jsonString);
    }
}

