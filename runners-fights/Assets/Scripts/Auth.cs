using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Networking;

public class Auth : MonoBehaviour
{
    [SerializeField] InputField email;
    [SerializeField] InputField password;

    [SerializeField] Text errorMessages;
    //[SerializeField] GameObject progressCircle;

    [SerializeField] Button loginButton;
    public Text buttonText;
    public ConnectToServer connecToPhoton;

    WWWForm form;

    public void OnLoginButtonClicked()
    {
        if (email.text.Length > 1 && password.text.Length > 0) {
            //disabled el boton hasta que rellene campos
            loginButton.interactable = false;
            buttonText.text = "Conectando...";
            //progressCircle.SetActive(true);


            //TODO url de la api
            //StartCoroutine(Login());

            //Temporal until StartCoroutine(Login()) is terminated
            connecToPhoton.OnClickConnect(email.text);
        }
    }

    IEnumerator Login()
    {
        form = new WWWForm();

        form.AddField("email", email.text);
        form.AddField("password", password.text);

        UnityWebRequest www = UnityWebRequest.Post(/*TODO url de la api*/ "", form);

        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
            errorMessages.text = www.error;
            buttonText.text = "Conectar";
        }
        else
        {
            Debug.Log("Form upload complete!");
            connecToPhoton.OnClickConnect(email.text);
        }

        loginButton.interactable = true;
        //progressCircle.SetActive(false);
    }
}
