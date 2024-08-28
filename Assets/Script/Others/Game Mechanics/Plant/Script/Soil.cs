using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Soil : MonoBehaviour
{
    public bool isEmpty = true;
    public bool playerInRange;
    public bool isLocked = false;
    [SerializeField] public Item requiredSeed;
    [SerializeField] private string plantPrefabName;
    [SerializeField] public Material defaultMaterial;
    [SerializeField] public Material wateredMaterial;

    public Plant currentPlant;
    public GameObject currentPlantObject;

    [SerializeField] private GameObject canvasPrefab; // Referensi ke prefab canvas
    private GameObject canvasInstance; // Instance dari canvas world space

    private Camera mainCamera; // Referensi ke kamera utama

    // Variabel publik untuk posisi canvas
    public Vector3 canvasPositionOffset = new Vector3(0, 1, 0);

    private void Start()
    {
        // Instantiate the canvas and set it as a child of this soil
        canvasInstance = Instantiate(canvasPrefab, transform);
        UpdateCanvasPosition(); // Update the position of the canvas

        mainCamera = Camera.main; // Inisialisasi referensi ke kamera utama
    }

    private void Update()
    {
        if (!isLocked)
        {
            float distance = Vector3.Distance(Player.instance.transform.position, transform.position);

            if (distance < 10f)
            {
                playerInRange = true;
            }
            else
            {
                playerInRange = false;
            }
        }
        if (currentPlant != null && currentPlant.isOneTimeHarvest && currentPlant.maturePlantModel == null)
        {
            ClearSoil();
        }
        else if (currentPlant != null && currentPlant.isOneTimeHarvest && currentPlant.produce && currentPlant.isOneTimeHarvestWithManys && currentPlant.isHasfruitedonce && currentPlant.CurrentProduceCount == 0)
        {
            ClearSoil();
        }

        // Membuat canvas selalu menghadap kamera
        if (canvasInstance != null)
        {
            canvasInstance.transform.LookAt(mainCamera.transform);
            canvasInstance.transform.Rotate(0, 180, 0); // Rotasi 180 derajat di sumbu Y
        }

        UpdateCanvasUI();
    }

    internal void OnConfirmButtonClicked()
    {
        if (isLocked)
        {
            Item selectedItem = InventoryManager.instance.GetSelectedItem(false);

            if (selectedItem == requiredSeed && isEmpty)
            {
                InventoryManager.instance.LockSlotChange(true); // Mengunci perubahan slot saat menanam
                Player.instance.PlantSeed();
            }
        }
    }

    internal void PlantSeed()
    {
        isEmpty = false;

        GameObject instantiatedPlant = Instantiate(Resources.Load(plantPrefabName) as GameObject, transform);
        // Ubah path prefab sesuai dengan struktur folder di Resources kalau ada di folder lain.
        // string prefabPath = "Plant/Prefabs/" + plantPrefabName; // Misal folder Resources/Plant/Prefabs/(file prefabs dari plant yang mau ditanam).
        // GameObject instantiatedPlant = Instantiate(Resources.Load(prefabPath) as GameObject, transform);

        Vector3 plantPosition = Vector3.zero;
        plantPosition.y = 0f;
        instantiatedPlant.transform.localPosition = plantPosition;

        InventoryManager.instance.ConsumeItem(requiredSeed);
        InventoryManager.instance.LockSlotChange(false); // Mengunci perubahan slot saat menanam

        currentPlant = instantiatedPlant.GetComponent<Plant>();
        currentPlant.secondsOfPlanting = Time.time;
        currentPlantObject = instantiatedPlant; // Set the reference to the plant GameObject

        UpdateCanvasPosition(); // Update the position of the canvas when a seed is planted
        UpdateCanvasUI(); // Memperbarui UI canvas saat benih ditanam
    }

    public void UpdateCanvasUI()
    {
        // Image border = canvasInstance.transform.Find("Border").GetComponent<Image>(); 
        Image background = canvasInstance.transform.Find("Background").GetComponent<Image>();

        Image plantImageUI = canvasInstance.transform.Find("PlantImage").GetComponent<Image>();
        TMP_Text seedTextUI = canvasInstance.transform.Find("SeedText").GetComponent<TMP_Text>();

        if (background != null && plantImageUI != null && seedTextUI != null)
        {
            // border.gameObject.SetActive(true); 
            background.gameObject.SetActive(true);

            if (isEmpty)
            {
                plantImageUI.sprite = requiredSeed.image;
                seedTextUI.text = $"Required Seed: {requiredSeed.name}";
                seedTextUI.gameObject.SetActive(isEmpty);
                canvasInstance.transform.localPosition = canvasPositionOffset;
            }
            else if (!isEmpty && currentPlant.isWatered)
            {
                plantImageUI.sprite = currentPlant.plantImage;
                seedTextUI.text = $"{currentPlant.PlantName}";
                seedTextUI.gameObject.SetActive(!isEmpty && currentPlant.isWatered);
            }
            else if (!isEmpty && currentPlant.produce && !currentPlant.waterForFruit)
            {
                plantImageUI.sprite = currentPlant.plantImage;
                seedTextUI.text = $"Ambil : {currentPlant.PlantName}";
                seedTextUI.gameObject.SetActive(!isEmpty && currentPlant.produce && !currentPlant.waterForFruit);
            }
            else if (!isEmpty && currentPlant.produce && currentPlant.waterForFruit && currentPlant.CurrentProduceCount >= 1)
            {
                plantImageUI.sprite = currentPlant.plantImage;
                seedTextUI.text = $"Ambil : {currentPlant.PlantName}";
                seedTextUI.gameObject.SetActive(!isEmpty && currentPlant.produce && currentPlant.waterForFruit && currentPlant.CurrentProduceCount >= 1);
            }
            else if (!isEmpty && !currentPlant.produce)
            {
                plantImageUI.sprite = PlantManager.instance.waterImage;
                seedTextUI.text = $"Need Water";
                seedTextUI.gameObject.SetActive(!isEmpty && !currentPlant.produce);
            }
            else if (!isEmpty && currentPlant.produce && currentPlant.waterForFruit && currentPlant.CurrentProduceCount == 0)
            {
                plantImageUI.sprite = PlantManager.instance.waterImage;
                seedTextUI.text = $"Need Water";
                seedTextUI.gameObject.SetActive(!isEmpty && currentPlant.produce && currentPlant.waterForFruit && currentPlant.CurrentProduceCount == 0);
            }
        }
    }

    public void UpdateCanvasPosition()
    {
        if (currentPlantObject != null && currentPlant != null)
        {
            float plantHeight = currentPlant.GetHeightForCurrentStage();
            canvasInstance.transform.localPosition = new Vector3(0, plantHeight, 0);
        }
        else if (isEmpty)
        {
            canvasInstance.transform.localPosition = canvasPositionOffset; // Default position if no plant is present
        }
    }



    public void ClearSoil()
    {
        if (currentPlantObject != null)
        {
            Destroy(currentPlantObject);
        }
        isEmpty = true;
        currentPlantObject = null;
        currentPlant = null;
        canvasInstance.transform.localPosition = canvasPositionOffset;
        UpdateCanvasPosition(); // Update the position of the canvas when soil is cleared
        UpdateCanvasUI(); // Memperbarui UI canvas saat tanah kosong
    }

    internal void MakeSoilWatered()
    {
        GetComponent<Renderer>().material = wateredMaterial;
    }

    internal void MakeSoilNotWatered()
    {
        GetComponent<Renderer>().material = defaultMaterial;
    }

    public void LockSoil()
    {
        isLocked = true;
    }

    public void UnlockSoil()
    {
        isLocked = false;
    }

    public string GetRequiredSeedName()
    {
        return requiredSeed.name;
    }

    public void SetCurrentPlant(GameObject plant)
    {
        currentPlantObject = plant;
    }
}