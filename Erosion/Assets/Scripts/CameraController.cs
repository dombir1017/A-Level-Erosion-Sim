using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed;
    public MeshRenderer terrain;

    void Update() //Called once per frame, moves camera right, left, forwards, or backwards when arrow key pressed and makes it look at the centre of the terrain
    {
        if(Input.GetKey(KeyCode.UpArrow)){
            transform.position += transform.forward * Time.deltaTime * cameraSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= transform.forward * Time.deltaTime * cameraSpeed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position -= transform.right * Time.deltaTime * cameraSpeed;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += transform.right * Time.deltaTime * cameraSpeed;
        }
        transform.LookAt(terrain.bounds.center);
    }
}
