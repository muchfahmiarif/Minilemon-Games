using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform target; // Your player character
    public float distance = 5.0f;
    public float height = 2.0f;
    public float smoothSpeed = 0.125f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float maxLookAngle = 80f;
    private float rotationX = 0f;

    private Vector3 offset;

    void Start()
    {
        offset = new Vector3(0, height, -distance);
    }

    void FixedUpdate()  // Use FixedUpdate for smoother camera movement
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxLookAngle, maxLookAngle);

        transform.LookAt(target);
        transform.Rotate(Vector3.up * mouseX);

    }
}
