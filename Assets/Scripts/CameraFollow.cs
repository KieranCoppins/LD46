using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject turtle;
    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - turtle.transform.position;
    }

    void LateUpdate()
    {
        transform.position = turtle.transform.position + offset;
    }
}
