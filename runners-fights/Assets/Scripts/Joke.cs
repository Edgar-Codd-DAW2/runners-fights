using UnityEngine;

[System.Serializable]
public class Joke
{
    public string value;

    public string GetJoke()
    {
        return value;
    }

    public static Joke CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Joke>(jsonString);
    }
}
