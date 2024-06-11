using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float jumpForce = 10f; // Besar gaya loncatan

    private Rigidbody rb;

    void Start()
    {
        PrintInstruction();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody tidak ditemukan pada objek!");
        }
    }

    void Update()
    {
        MovePlayer();
        RotatePlayer();

        // Cek input sentuh
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void PrintInstruction()
    {
        Debug.Log("Welcome to the game");
    }

    void Jump()
    {
        // Menerapkan gaya loncatan pada Rigidbody
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void MovePlayer()
    {
        float xValue = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float zValue = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(xValue, 0, zValue);
    }

    void RotatePlayer()
    {

        
    }
}