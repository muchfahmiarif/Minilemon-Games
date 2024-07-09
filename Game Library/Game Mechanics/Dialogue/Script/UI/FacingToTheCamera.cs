using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingToTheCamera : MonoBehaviour
{
    private Camera mainCamera;
    
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        transform.LookAt(mainCamera.transform);
        transform.Rotate(0, 180, 0);
        
    }
}
