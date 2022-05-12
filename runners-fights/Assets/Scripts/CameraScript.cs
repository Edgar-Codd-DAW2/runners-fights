using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    

    public GameObject personaje;
    private Vector3 posicionRelativa;

    void Start()
    {

        posicionRelativa = transform.position - personaje.transform.position;

    }
    void LateUpdate()
    {

        transform.position = personaje.transform.position + posicionRelativa;

    }
}
