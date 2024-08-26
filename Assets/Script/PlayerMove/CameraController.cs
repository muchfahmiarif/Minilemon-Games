using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float sensitivity = -1f; 
    private float rotatespeed;
    private Vector3 rotate;
    // Start is called before the first frame update
    void Start()
    { 
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
       // float y = Input.GetAxis("Mouse X");
        //target.Rotate(0, y, 0);
        
        //float x = Input.GetAxis("Mouse Y");
       // target.Rotate(x, 0, 0);

       // rotate = new Vector3 (x, y*sensitivity, 0);
        transform.eulerAngles -= rotate;
    }
}
