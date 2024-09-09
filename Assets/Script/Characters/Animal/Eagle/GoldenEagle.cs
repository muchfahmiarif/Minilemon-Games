using UnityEngine;

public class GoldenEagle : MonoBehaviour
{
    [SerializeField] private Transform pos1, pos2;
    [SerializeField] private Animator animator;

    private bool isFlying;

    private void Update()
    {
        if (isFlying)
            Fly();
    }

    private void Fly()
    {
        Quaternion Look = Quaternion.LookRotation(pos2.position - transform.position);
        transform.rotation = Look;
        transform.position = Vector3.MoveTowards(transform.position, pos2.position, Time.deltaTime * 8);
        if (Vector3.Distance(transform.position, pos2.position) <= 0.1f)
        {
            animator.Play("Idle_A");
            isFlying = false;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            animator.Play("Fly");
            isFlying = true;
        }
    }
}