using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plataformasAtraveables : MonoBehaviour
{
    private PlatformEffector2D effector;
    public float tiempoEspera;

    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            tiempoEspera = 0.5f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            if(tiempoEspera <= 0)
            {
                effector.rotationalOffset = 180f;
                tiempoEspera = 0.5f;
            }
            else
            {
                tiempoEspera -= Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            effector.rotationalOffset = 0;
        }
    }
}
