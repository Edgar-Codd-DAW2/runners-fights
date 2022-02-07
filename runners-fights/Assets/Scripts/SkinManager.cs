using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SkinManager : MonoBehaviour
{

    public SpriteRenderer sr;
    public List<Sprite> skins = new List<Sprite>();
    private int selectedSkin = 0;
    public GameObject playerSkin;

    public void nextOption()
    {
        selectedSkin = selectedSkin + 1;
        if(selectedSkin == skins.Count)
        {
            selectedSkin = 0;
        }
        sr.sprite = skins[selectedSkin];
    }

    public void backOption()
    {
        selectedSkin = selectedSkin - 1;
        if (selectedSkin <0)
        {
            selectedSkin = skins.Count -1;
        }
        sr.sprite = skins[selectedSkin];
    }

    public void playGame()
    {
        //PrefabUtility.SaveAsPrefabAsset(playerSkin, "Assets/Sprites/SkinSelector/selectedSkin.prefab");
        //SceneManager.LoadScene("MapaTutorial");
    }
}
