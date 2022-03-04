using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaEnMovimiento : MonoBehaviour
{

    public GameObject ObjectoAmover;
    public Transform StartPoint;
    public Transform EndPoint;

    public float Velocidad;

    private Vector3 MoverHacia;

    // Start is called before the first frame update
    void Start()
    {
        MoverHacia = EndPoint.position;    
    }

    // Update is called once per frame
    void Update()
    {
        ObjectoAmover.transform.position = Vector3.MoveTowards(ObjectoAmover.transform.position, MoverHacia, Velocidad * Time.deltaTime);
        if(ObjectoAmover.transform.position == EndPoint.position)
        {
            MoverHacia = StartPoint.position;
        }
        if (ObjectoAmover.transform.position == StartPoint.position)
        {
            MoverHacia = EndPoint.position;
        }
    }
}
