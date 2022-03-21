using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleports : MonoBehaviour
{
   [SerializeField] private Transform destino;

    public Transform getDestino()
    {
        return destino;
    }
}
