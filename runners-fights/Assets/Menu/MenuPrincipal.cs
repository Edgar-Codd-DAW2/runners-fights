using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    } 
    public void EmpezarModoHistoria()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void EmpezarTutorial()
    {
        //Debug.Log("Clickas A Primer Mapa");
        SceneManager.LoadScene("MapaTutorial");
    }

    /*public void EmpezarMultiplayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
    public void Options()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }*/
    public void CerrarJuego()
    {
        Application.Quit();
        Debug.Log("Salir");
    }
}
