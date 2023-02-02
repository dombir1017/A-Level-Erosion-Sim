using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed;
    void Update() //Called once per frame
    {
        if(Input.GetKey(KeyCode.W)){
            transform.position += transform.forward * Time.deltaTime * cameraSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * cameraSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * Time.deltaTime * cameraSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Time.deltaTime * cameraSpeed;
        }
        transform.LookAt(new Vector3(50, 0, 50));
    }
}
