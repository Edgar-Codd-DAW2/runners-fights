using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuPrincipal : MonoBehaviour
{
    // Start is called before the first frame update
    public void EmpezarModoHistoria()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void EmpezarTutorial()
    {
        //Debug.Log("Clickas A Primer Mapa");
        SceneManager.LoadScene("MapaTutorial");
    }
    public void PrimerNivel()
    {
        //Debug.Log("Clickas A Primer Mapa");
        SceneManager.LoadScene("Map1Lvl1");
    }
    public void SegundoNivel()
    {
        //Debug.Log("Clickas A Primer Mapa");
        SceneManager.LoadScene("mapa roger 1");
    }
    public void EmpezarMultiplayer()
    {
        SceneManager.LoadScene("Lobby");
    }
    public void Multiplayer()
    {
        SceneManager.LoadScene("Stadiummulti1");
    }

    public void VolverMenuPrincipal()
    {
        SceneManager.LoadScene("Menu");
    }

    public void CerrarJuego()
    {
        Application.Quit();
        Debug.Log("Salir");
    }
}
