using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed;

	void Update ()
    {
        this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
	}
}
