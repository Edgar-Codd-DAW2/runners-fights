using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    Button[] LevelButons;
    // Start is called before the first frame update
    private void Awake()
        {
        int ReachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);
        if(PlayerPrefs.GetInt("Level")>= 2)
        {
            ReachedLevel = PlayerPrefs.GetInt("Level");
        }
        LevelButons = new Button[transform.childCount];
        for(int i=0;i<LevelButons.Length;i++)
        {
            LevelButons[i] = transform.GetChild(i).GetComponent<Button>();
            LevelButons[i].GetComponentInChildren<Text>().text = (i + 1).ToString();
            if(i+1> ReachedLevel)
            {
                LevelButons[i].interactable = false;
            }
        }
    }
    public void LoadScene(int Level)
    {
        PlayerPrefs.SetInt("Level", Level);
        Application.LoadLevel("Loading");

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
