using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using MalbersAnimations.Controller;
using MalbersAnimations;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Soil targetSoil;
    public bool isPlanting = false;

    private NavMeshAgent navMeshAgent;
    private MAnimal playerController;
    private Animator animator;  // Assuming you have an Animator for handling animations
    public float playerSpeed = 1.5f; // Public player speed
    public float stoppingDistance = 0.5f; // Public stopping distance

    [Header("Player Item Pickup")]
    public Transform handTransform;
    public GameObject currentItem;
    public float pickupRange = 2f;
    public LayerMask itemLayer;
    public float maxAngle = 60f;
    private Button buttonPickUp;
    private ItemPickup nearbyItemPickup;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        playerController = GetComponent<MAnimal>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        buttonPickUp = GameObject.FindGameObjectWithTag("PickUp").GetComponent<Button>();
        buttonPickUp.onClick.AddListener(PickUp);
        buttonPickUp.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckForNearbyItems();
        if (currentItem != null && InventoryManager.instance.IsSelectedSlotEmpty())
        {
            DropItem();
        }

        LockNearestSoil();

        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            SmoothMove();
        }
    }



    //Item related code
    void CheckForNearbyItems()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange, itemLayer);
        nearbyItemPickup = null;
        float minDistance = float.MaxValue;
        float maxDotProduct = -1.0f; // Ubah menjadi -1 untuk menjamin prioritas pada item yang lebih lurus

        foreach (Collider collider in hitColliders)
        {
            ItemPickup itemPickup = collider.GetComponent<ItemPickup>();
            if (itemPickup != null && itemPickup.gameObject != currentItem && !itemPickup.isInInventory)
            {
                Vector3 directionToItem = (collider.transform.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(transform.forward, directionToItem);
                float distance = Vector3.Distance(transform.position, collider.transform.position);

                // Hitung sudut antara arah pandang player dan arah ke item
                float angleToItem = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

                // Prioritaskan item yang pandangannya lebih lurus dengan player dan lebih dekat
                if (dotProduct > 0.4f && distance < minDistance && angleToItem <= maxAngle)
                {
                    minDistance = distance;
                    maxDotProduct = dotProduct;
                    nearbyItemPickup = itemPickup;
                }
                // Jika item memiliki dot product yang hampir sama dan lebih dekat, prioritas item yang lebih dekat
                else if (dotProduct > 0.4f && Mathf.Approximately(dotProduct, maxDotProduct) && distance < minDistance && angleToItem <= maxAngle)
                {
                    minDistance = distance;
                    nearbyItemPickup = itemPickup;
                }
            }
        }

        if (nearbyItemPickup != null && nearbyItemPickup.gameObject != currentItem)
        {
            buttonPickUp.gameObject.SetActive(true);
            buttonPickUp.interactable = true;
        }
        else
        {
            buttonPickUp.gameObject.SetActive(false);
        }
    }

    void PickUp()
    {
        if (nearbyItemPickup != null && nearbyItemPickup.gameObject != currentItem && !nearbyItemPickup.isInInventory)
        {
            nearbyItemPickup.PickUp();
            buttonPickUp.gameObject.SetActive(false);

            // Set isInInventory ke true untuk item yang diambil
            nearbyItemPickup.isInInventory = true;
        }
    }

    public void HoldItem(Item item)
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        currentItem = Instantiate(item.prefab, handTransform);
        currentItem.transform.localPosition = item.holdPosition;
        currentItem.transform.localRotation = Quaternion.Euler(item.holdRotation);
        currentItem.transform.localScale = item.holdScale;
        currentItem.GetComponentInChildren<ItemPickup>().isInInventory = true;
    }

    public void DropItem()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
            currentItem.GetComponentInChildren<ItemPickup>().isInInventory = false;
        }
    }




    //Plant related code
    public void WaterPlant()
    {
        if (targetSoil != null && targetSoil.currentPlant != null)
        {
            StartCoroutine(WaterPlantWithTimeout());
        }
    }

    private IEnumerator WaterPlantWithTimeout()
    {
        float timeoutDuration = 2f; // Durasi timeout dalam detik

        // Start the watering routine
        StartCoroutine(WaterPlantRoutine());

        // Wait for the timeout duration
        yield return new WaitForSeconds(timeoutDuration);

        // Check if the coroutine is still running and terminate if necessary
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            // Remove NavMeshAgent and enable player controller
            Destroy(navMeshAgent);
            playerController.enabled = true;

            // Reset animations if they are still playing
            animator.ResetTrigger("Walk");
            animator.ResetTrigger("Watering");

            Debug.Log("WaterPlant routine terminated due to timeout.");
        }
    }


    private IEnumerator WaterPlantRoutine()
    {
        // Disable player controller and add NavMeshAgent
        playerController.enabled = false;
        navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        navMeshAgent.speed = playerSpeed;
        navMeshAgent.stoppingDistance = stoppingDistance;

        // Perform walking animation
        animator.SetTrigger("Walk");

        // Move towards the plant
        navMeshAgent.SetDestination(targetSoil.transform.position);
        yield return new WaitUntil(() => !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance);

        // Stop walking animation
        animator.ResetTrigger("Walk");

        // Rotate player to face the target soil
        Vector3 lookDirection = targetSoil.transform.position - transform.position;
        lookDirection.y = 0f; // Keep the rotation on the X-Z plane
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);

        // Perform watering animation
        animator.SetTrigger("Watering");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // After animation, mark soil as watered
        targetSoil.currentPlant.WaterPlant();
        targetSoil.MakeSoilWatered();

        // Remove NavMeshAgent and enable player controller
        Destroy(navMeshAgent);
        playerController.enabled = true;
    }

    public void PlantSeed()
    {
        if (isPlanting || targetSoil == null || !targetSoil.isEmpty || InventoryManager.instance.IsSelectedSlotEmpty())
        {
            return; // Jangan lakukan apa pun jika sedang menanam, tanah tidak kosong, atau slot terpilih kosong
        }
        if (targetSoil != null && targetSoil.isEmpty && InventoryManager.instance.GetSelectedItem(false) == targetSoil.requiredSeed)
        {
            // Debug.Log("Found soil: " + targetSoil.name);
            isPlanting = true; // Mulai menanam
            StartCoroutine(PlantSeedWithTimeout());
        }
    }

    private IEnumerator PlantSeedWithTimeout()
    {
        float timeoutDuration = 2f; // Durasi timeout dalam detik

        // Start the planting routine
        StartCoroutine(PlantSeedRoutine());

        // Wait for the timeout duration
        yield return new WaitForSeconds(timeoutDuration);

        // Setelah menunggu selesai, nonaktifkan flag isPlanting
        isPlanting = false;

        // Check if the coroutine is still running and terminate if necessary
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            // Remove NavMeshAgent and enable player controller
            Destroy(navMeshAgent);
            playerController.enabled = true;

            // Reset animations if they are still playing
            animator.ResetTrigger("Walk");
            animator.ResetTrigger("plantingSeed");

            Debug.Log("PlantSeed routine terminated due to timeout.");
        }
    }

    private IEnumerator PlantSeedRoutine()
    {
        // Disable player controller and add NavMeshAgent
        playerController.enabled = false;
        navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        navMeshAgent.speed = playerSpeed;
        navMeshAgent.stoppingDistance = stoppingDistance;

        // Perform walking animation
        animator.SetTrigger("Walk");

        // Move towards the soil
        navMeshAgent.SetDestination(targetSoil.transform.position);
        yield return new WaitUntil(() => !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance);

        // Stop walking animation
        animator.ResetTrigger("Walk");

        // Rotate player to face the target soil
        Vector3 lookDirection = targetSoil.transform.position - transform.position;
        lookDirection.y = 0f; // Keep the rotation on the X-Z plane
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f);

        // Perform planting animation
        animator.SetTrigger("plantingSeed");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Plant the seed
        targetSoil.PlantSeed();

        // Remove NavMeshAgent and enable player controller
        Destroy(navMeshAgent);
        playerController.enabled = true;
    }

    private void LockNearestSoil()
    {
        Soil[] allSoils = FindObjectsOfType<Soil>();
        Soil nearestSoil = null;
        float minDistance = float.MaxValue;

        foreach (Soil soil in allSoils)
        {
            float distance = Vector3.Distance(transform.position, soil.transform.position);
            Vector3 directionToSoil = (soil.transform.position - transform.position).normalized;
            if (distance < minDistance && Vector3.Dot(transform.forward, directionToSoil) > 0.5f)
            {
                minDistance = distance;
                nearestSoil = soil;
            }
        }

        if (nearestSoil != null)
        {
            if (targetSoil != null && targetSoil != nearestSoil)
            {
                targetSoil.UnlockSoil();
            }

            targetSoil = nearestSoil;
            targetSoil.LockSoil();
            PlantManager.instance.UpdateUIForSoil(targetSoil);
        }
    }

    private void SmoothMove()
    {
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            Vector3 desiredPosition = navMeshAgent.nextPosition;
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, navMeshAgent.speed * Time.deltaTime);
            Quaternion desiredRotation = Quaternion.LookRotation(navMeshAgent.desiredVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * 5.0f);
        }
    }
}
