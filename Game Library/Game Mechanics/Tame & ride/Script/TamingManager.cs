using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using MalbersAnimations.Controller;
using MalbersAnimations;
using UnityEngine.AI;
using System.Collections.Generic;

public class TamingManager : MonoBehaviour
{
    [System.Serializable]
    public class AnimalSlot
    {
        public GameObject animal;
        public bool isTamed = false;
        public bool isCanRide = true; // Menambahkan bool isCanRide
        public bool inRide = false; // Menambahkan bool inRide
        public Item[] tamingRequirements; // Mengganti string[] dengan Item[]
        public Transform givePoint;
        public float animalFollowSpeed = 2.0f;
        public float followDistance = 2.0f;
        public float maxFollowDistance = 10.0f;
        public float maxAnimalProximity = 2.0f;
        public Transform followPoint;
        public bool isminFlyingHeight = false;
        public bool isFlyingAnimal = false; // Apakah animal bisa terbang
        public float minFlyingHeight = 5.0f; // Ketinggian minimal animal terbang
        public NavMeshAgent animalNavMeshAgent; // NavMeshAgent untuk animal
    }

    [SerializeField] private GameObject player;
    [SerializeField] private Button mountButton;
    [SerializeField] private Button dismountButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI requirementsText;
    [SerializeField] private TextMeshProUGUI messageText; // Pesan batas maksimal
    [SerializeField] private float messageDisplayDuration = 0.5f; // Durasi messageText tampil
    [SerializeField] private float fadeDuration = 0.25f; // Durasi animasi fade in/out

    [SerializeField] private int maxTameableAnimals = 14; // Batas maksimal hewan yang bisa di-tame oleh pemain
    [SerializeField] private int tamedAnimalCount = 0; // Jumlah hewan yang sudah di-tame oleh pemain
    public float playerSpeed = 1.5f;

    [SerializeField] private List<Transform> followPoints;

    [SerializeField] private AnimalSlot[] animalSlots;

    private AnimalSlot currentAnimalSlot;
    private AnimalSlot tamedAnimalSlot;
    private Animator playerAnimator;
    private MAnimal playerController;
    private NavMeshAgent playerNavMeshAgent;
    private bool isMovingToGivePoint = false;
    private bool haveItems = false;

    void Start()
    {
        mountButton.gameObject.SetActive(false);
        dismountButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        requirementsText.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false); // Sembunyikan pesan batas maksimal di awal
        playerAnimator = player.GetComponent<Animator>();
        playerController = player.GetComponent<MAnimal>();

        mountButton.onClick.AddListener(OnMountButtonClicked);
        dismountButton.onClick.AddListener(OnDismountButtonClicked);
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
    }

    void Update()
    {
        bool showConfirmUI = false;
        bool showMountUI = false;
        AnimalSlot untamedAnimalSlot = null;
        tamedAnimalSlot = null;

        foreach (AnimalSlot slot in animalSlots)
        {
            if (Vector3.Distance(player.transform.position, slot.animal.transform.position) < 3.0f)
            {
                if (!slot.isTamed)
                {
                    untamedAnimalSlot = slot;
                }
                else
                {
                    tamedAnimalSlot = slot;
                }

                if (untamedAnimalSlot != null && tamedAnimalSlot != null)
                {
                    break;
                }
            }
        }

        if (untamedAnimalSlot != null)
        {
            currentAnimalSlot = untamedAnimalSlot;
            showConfirmUI = true;
        }

        if (tamedAnimalSlot != null && tamedAnimalSlot.inRide)
        {
            dismountButton.gameObject.SetActive(true);
        }

        if (tamedAnimalSlot != null && tamedAnimalSlot.isCanRide) // Hanya tampilkan UI mount untuk hewan yang bisa dinaiki
        {
            showMountUI = true;
        }

        if (showConfirmUI || showMountUI)
        {
            ShowTamingUI(showConfirmUI, showMountUI);
        }
        else
        {
            HideTamingUI();
        }

        if (isMovingToGivePoint && currentAnimalSlot != null)
        {
            if (!playerNavMeshAgent.pathPending && playerNavMeshAgent.remainingDistance < 0.1f)
            {
                isMovingToGivePoint = false;
                playerAnimator.SetTrigger("Give");

                Vector3 directionToAnimal = (currentAnimalSlot.animal.transform.position - player.transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToAnimal.x, 0, directionToAnimal.z));
                player.transform.rotation = lookRotation;

                StartCoroutine(HandleTamingProcess());
            }
        }

        foreach (AnimalSlot slot in animalSlots)
        {
            if (slot.isTamed && slot.isCanRide)
            {
                bool isMounted = IsAnimalBeingMounted(slot.animal);
                if (!slot.inRide && isMounted) // Jika animal belum di naiki dan MalbersInput aktif
                {
                    slot.inRide = true;
                    // Lakukan tindakan yang diperlukan saat animal di naiki
                }
                else if (slot.inRide && !isMounted) // Jika animal sedang di naiki tetapi MalbersInput tidak aktif
                {
                    slot.inRide = false;
                    // Lakukan tindakan yang diperlukan saat animal tidak lagi di naiki
                }
            }
        }

        foreach (AnimalSlot slot in animalSlots)
        {
            if (slot.isTamed && slot.inRide)
            {
                NavMeshAgent animalNavMeshAgent = slot.animal.GetComponent<NavMeshAgent>();
                if (animalNavMeshAgent != null)
                {
                    animalNavMeshAgent.enabled = false;
                    dismountButton.gameObject.SetActive(true);
                }
            }
            if (slot.isTamed && !slot.inRide)
            {
                NavMeshAgent animalNavMeshAgent = slot.animal.GetComponent<NavMeshAgent>();
                if (animalNavMeshAgent != null)
                {
                    animalNavMeshAgent.enabled = true;
                }
            }
        }

        foreach (AnimalSlot slot in animalSlots)
        {
            if (slot.isTamed && !slot.inRide)
            {
                if (slot.isFlyingAnimal)
                {
                    MoveFlyingAnimalTowardsPlayer(slot);
                }
                else
                {
                    MoveAnimalTowardsPlayer(slot);
                }
                MaintainAnimalProximity(slot);
            }
        }
    }

    private bool IsAnimalBeingMounted(GameObject animal)
    {
        MalbersInput malbersInput = animal.GetComponent<MalbersInput>();
        // Periksa apakah komponen MalbersInput ada dan aktif pada animal
        return malbersInput != null && malbersInput.enabled;
    }

    private void ShowTamingUI(bool showConfirm, bool showMount)
    {
        confirmButton.gameObject.SetActive(showConfirm);
        mountButton.gameObject.SetActive(showMount);
        dismountButton.gameObject.SetActive(showMount);
        requirementsText.gameObject.SetActive(showConfirm);

        if (showConfirm && currentAnimalSlot != null && !currentAnimalSlot.isTamed)
        {
            if (currentAnimalSlot.tamingRequirements.Length == 0)
            {
                requirementsText.text = "No items required for taming.";
            }
            else
            {
                // Convert Item[] to string[]
                string[] requirementNames = new string[currentAnimalSlot.tamingRequirements.Length];
                for (int i = 0; i < currentAnimalSlot.tamingRequirements.Length; i++)
                {
                    requirementNames[i] = currentAnimalSlot.tamingRequirements[i].name;
                }

                // Use string.Join with the string[] array
                requirementsText.text = "Requirements: " + string.Join(", ", requirementNames);

            }

            // Set button transparency based on whether the maximum tamed animals count has been reached
            Color buttonColor = confirmButton.image.color;
            buttonColor.a = tamedAnimalCount < maxTameableAnimals ? 1f : 0.5f;
            confirmButton.image.color = buttonColor;
        }
    }

    private void HideTamingUI()
    {
        mountButton.gameObject.SetActive(false);
        dismountButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        requirementsText.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false); // Sembunyikan pesan batas maksimal saat UI taming disembunyikan
    }

    private IEnumerator ShowMessageText(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            SetMessageTextAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetMessageTextAlpha(1f);

        // Display for a duration
        yield return new WaitForSeconds(messageDisplayDuration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            SetMessageTextAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetMessageTextAlpha(0f);

        messageText.gameObject.SetActive(false);
    }

    private void SetMessageTextAlpha(float alpha)
    {
        Color color = messageText.color;
        color.a = alpha;
        messageText.color = color;
    }


    public void OnConfirmButtonClicked()
    {
        if (currentAnimalSlot != null && !currentAnimalSlot.isTamed)
        {
            if (tamedAnimalCount < maxTameableAnimals)
            {
                
                Item selectedItem = InventoryManager.instance.GetSelectedItem(false);
                foreach (Item item in currentAnimalSlot.tamingRequirements)
                {
                    if (selectedItem == item)
                    {
                        haveItems = true;
                    }
                    else if (selectedItem != item)
                    {
                        haveItems = false;
                    }
                }
                if (currentAnimalSlot.tamingRequirements.Length == 0 || haveItems)
                {
                    if (haveItems)
                    {
                        foreach (Item item in currentAnimalSlot.tamingRequirements)
                        {
                            InventoryManager.instance.ConsumeItem(item);
                        }
                    }

                    playerNavMeshAgent = player.GetComponent<NavMeshAgent>();
                    if (playerNavMeshAgent == null)
                    {
                        playerNavMeshAgent = player.AddComponent<NavMeshAgent>();
                    }

                    playerNavMeshAgent.speed = playerSpeed;
                    playerNavMeshAgent.SetDestination(currentAnimalSlot.givePoint.position);

                    isMovingToGivePoint = true;
                    playerController.enabled = false;
                    playerAnimator.SetTrigger("Walk");
                }
                else
                {
                    Debug.Log("Player does not have the required items for taming.");
                }
            }
            else
            {
                // Show message if max tamed animals limit is reached
                StartCoroutine(ShowMessageText("Sudah mencapai batas hewan yang bisa di tame yaitu " + maxTameableAnimals + "/" + tamedAnimalCount));
            }
        }
    }

    private bool CheckPlayerHasRequirements(Item[] requirements)
    {
        Item selectedItem = InventoryManager.instance.GetSelectedItem(false);
        foreach (Item item in requirements)
        {
            if (selectedItem != item)
            {
                return false;
            }
        }
        return true;
    }

    private IEnumerator HandleTamingProcess()
    {
        yield return new WaitForSeconds(2);
        currentAnimalSlot.isTamed = true;
        tamedAnimalCount++; // Increment the count of tamed animals
        mountButton.gameObject.SetActive(currentAnimalSlot.isCanRide); // Hanya tampilkan tombol mount jika hewan bisa dinaiki
        dismountButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        requirementsText.gameObject.SetActive(false);

        // Tambahkan NavMeshAgent ke AnimalSlot
        currentAnimalSlot.animalNavMeshAgent = currentAnimalSlot.animal.GetComponent<NavMeshAgent>();
        if (currentAnimalSlot.animalNavMeshAgent == null)
        {
            currentAnimalSlot.animalNavMeshAgent = currentAnimalSlot.animal.AddComponent<NavMeshAgent>();
        }

        AddNavMeshToAnimal(currentAnimalSlot.animal);

        Animator animalAnimator = currentAnimalSlot.animal.GetComponent<Animator>();
        if (animalAnimator != null)
        {
            animalAnimator.SetTrigger("Happy");
        }

        playerController.enabled = true;

        if (playerNavMeshAgent != null)
        {
            Destroy(playerNavMeshAgent);
        }
    }

    private void MoveAnimalTowardsPlayer(AnimalSlot slot)
    {
        if (slot.animal != null)
        {
            NavMeshAgent animalNavMeshAgent = slot.animal.GetComponent<NavMeshAgent>();
            if (animalNavMeshAgent == null)
            {
                animalNavMeshAgent = slot.animal.AddComponent<NavMeshAgent>();
                animalNavMeshAgent.speed = slot.animalFollowSpeed;
            }
            else
            {
                animalNavMeshAgent.speed = slot.animalFollowSpeed;
            }

            float distanceToPlayer = Vector3.Distance(player.transform.position, slot.animal.transform.position);
            if (distanceToPlayer > slot.maxFollowDistance)
            {
                animalNavMeshAgent.SetDestination(player.transform.position);
            }
            else if (distanceToPlayer > slot.followDistance)
            {
                if (slot.followPoint != null)
                {
                    animalNavMeshAgent.SetDestination(slot.followPoint.position);
                }
                else
                {
                    animalNavMeshAgent.SetDestination(player.transform.position - (player.transform.position - slot.animal.transform.position).normalized * slot.followDistance);
                }
            }

            // Update animal animation based on movement and turning
            Animator animalAnimator = slot.animal.GetComponent<Animator>();
            if (animalAnimator != null)
            {
                bool isMoving = animalNavMeshAgent.velocity.magnitude > 0.1f;
                animalAnimator.SetBool("IsWalking", isMoving);

                // Check if the animal is turning
                float angle = Vector3.Angle(slot.animal.transform.forward, animalNavMeshAgent.velocity.normalized);
                bool isTurning = angle > 0.3f; // Threshold angle to detect turning
                animalAnimator.SetBool("IsTurning", isTurning);
            }
        }
    }

    private void MoveFlyingAnimalTowardsPlayer(AnimalSlot slot)
    {
        if (slot.animal != null)
        {
            if (!slot.inRide && !slot.isCanRide) // Periksa apakah hewan tidak sedang dinaiki
            {
                NavMeshAgent animalNavMeshAgent = slot.animal.GetComponent<NavMeshAgent>();
                if (animalNavMeshAgent == null)
                {
                    animalNavMeshAgent = slot.animal.AddComponent<NavMeshAgent>();
                    animalNavMeshAgent.speed = slot.animalFollowSpeed;
                    animalNavMeshAgent.agentTypeID = -1372625422; // Set agent type to "FlyPath"
                }
                else
                {
                    animalNavMeshAgent.speed = slot.animalFollowSpeed;
                }

                float distanceToPlayer = Vector3.Distance(player.transform.position, slot.animal.transform.position);

                if (distanceToPlayer > slot.maxFollowDistance)
                {
                    animalNavMeshAgent.SetDestination(player.transform.position);
                    Animator animalAnimator = slot.animal.GetComponent<Animator>();
                    if (animalAnimator != null)
                    {
                        animalAnimator.SetBool("IsFlying", true);
                    }
                }
                else if (distanceToPlayer > slot.followDistance)
                {
                    if (slot.followPoint != null)
                    {
                        animalNavMeshAgent.SetDestination(slot.followPoint.position);
                        Animator animalAnimator = slot.animal.GetComponent<Animator>();
                        if (animalAnimator != null)
                        {
                            animalAnimator.SetBool("IsFlying", true);
                        }
                    }
                    else
                    {
                        animalNavMeshAgent.SetDestination(player.transform.position - (player.transform.position - slot.animal.transform.position).normalized * slot.followDistance);
                        Animator animalAnimator = slot.animal.GetComponent<Animator>();
                        if (animalAnimator != null)
                        {
                            animalAnimator.SetBool("IsFlying", true);
                        }
                    }
                }

                // Check if the animal has reached minimum flying height
                if (!slot.isminFlyingHeight)
                {
                    StartCoroutine(FlyToMinHeight(slot.animal, slot.minFlyingHeight));
                }
                else
                {
                    // Lock animal at the minimum flying height
                    LockAnimalHeight(slot.animal, slot.minFlyingHeight);
                }
            }
        }
    }

    private IEnumerator FlyToMinHeight(GameObject animal, float minHeight)
    {
        Animator animalAnimator = animal.GetComponent<Animator>();
        if (animalAnimator != null)
        {
            animalAnimator.SetTrigger("TakeOff");
        }

        Vector3 initialPosition = animal.transform.position;
        Vector3 targetPosition = initialPosition + Vector3.up * minHeight;
        float elapsedTime = 0f;
        float duration = 2f; // Adjust duration as needed for smoother movement

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            animal.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set the position directly to ensure it reaches exactly minHeight
        animal.transform.position = targetPosition;

        // Once reached the minimum flying height, set isminFlyingHeight to true
        AnimalSlot slot = FindAnimalSlot(animal);
        if (slot != null)
        {
            slot.isminFlyingHeight = true;
        }
    }

    private void LockAnimalHeight(GameObject animal, float lockHeight)
    {
        Vector3 lockedPosition = animal.transform.position;
        lockedPosition.y = lockHeight;
        animal.transform.position = lockedPosition;
    }

    private AnimalSlot FindAnimalSlot(GameObject animal)
    {
        foreach (AnimalSlot slot in animalSlots)
        {
            if (slot.animal == animal)
            {
                return slot;
            }
        }
        return null;
    }

    private void MaintainAnimalProximity(AnimalSlot slot)
    {
        foreach (AnimalSlot otherSlot in animalSlots)
        {
            if (otherSlot != slot && otherSlot.isTamed)
            {
                float distanceToOtherAnimal = Vector3.Distance(slot.animal.transform.position, otherSlot.animal.transform.position);
                if (distanceToOtherAnimal < slot.maxAnimalProximity)
                {
                    Vector3 directionAway = (slot.animal.transform.position - otherSlot.animal.transform.position).normalized;
                    Vector3 newPosition = slot.animal.transform.position + directionAway * (slot.maxAnimalProximity - distanceToOtherAnimal);
                    NavMeshAgent animalNavMeshAgent = slot.animal.GetComponent<NavMeshAgent>();
                    animalNavMeshAgent.SetDestination(newPosition);
                }
            }
        }
    }

    private void AddNavMeshToAnimal(GameObject animal)
    {
        AnimalSlot slot = FindAnimalSlot(animal);
        NavMeshAgent animalNavMeshAgent = animal.GetComponent<NavMeshAgent>();
        if (animalNavMeshAgent == null)
        {
            animalNavMeshAgent = animal.AddComponent<NavMeshAgent>();
        }
    }

    public void OnMountButtonClicked()
    {
        if (tamedAnimalSlot != null && tamedAnimalSlot.animal != null && !tamedAnimalSlot.isFlyingAnimal)
        {
            if (!tamedAnimalSlot.inRide && !tamedAnimalSlot.isFlyingAnimal) // Periksa apakah hewan belum sedang di naiki
            {
                SetNavMeshAgentActive(tamedAnimalSlot.animal, false);
                mountButton.gameObject.SetActive(false);
                dismountButton.gameObject.SetActive(true);
            }
        }
    }

    public void OnDismountButtonClicked()
    {
        if (tamedAnimalSlot != null && tamedAnimalSlot.animal != null && !tamedAnimalSlot.isFlyingAnimal)
        {
            if (tamedAnimalSlot.inRide && !tamedAnimalSlot.isFlyingAnimal) // Periksa apakah hewan sedang di naiki
            {
                SetNavMeshAgentActive(tamedAnimalSlot.animal, true);
                mountButton.gameObject.SetActive(true);
                dismountButton.gameObject.SetActive(false);
            }
        }
    }

    private void SetNavMeshAgentActive(GameObject animal, bool isActive)
    {
        AnimalSlot slot = FindAnimalSlot(animal);
        NavMeshAgent animalNavMeshAgent = slot.animal.GetComponent<NavMeshAgent>();
        if (animalNavMeshAgent != null)
        {
            animalNavMeshAgent.enabled = isActive;
        }
    }

}
