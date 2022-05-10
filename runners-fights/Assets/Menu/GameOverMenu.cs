using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public Text textJoke;

    void Start()
    {
        Time.timeScale = 1f;
        StartCoroutine(GetJokeCo());
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator GetJokeCo()
    {
        UnityWebRequest www = UnityWebRequest.Get("https://api.chucknorris.io/jokes/random");

        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Joke joke = Joke.CreateFromJSON(www.downloadHandler.text);
            textJoke.text = joke.GetJoke();
        }
    }
}
