using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantManager : MonoBehaviour
{
    public static PlantManager instance;

    public Button waterButton;
    // public TMP_Text seedText;
    public Button confirmButton;
    // public Image imagePlant; // Referensi untuk gambar item requiredSeed atau plantImage di UI
    public Sprite waterImage;
    [SerializeField] public Item waterCan;

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
    }

    private void Start()
    {
        waterButton.onClick.AddListener(() =>
        {
            Player.instance.WaterPlant();
        });
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        // seedText.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        waterButton.gameObject.SetActive(false);
        // imagePlant.gameObject.SetActive(false); // Sembunyikan gambar saat tidak dibutuhkan
    }

    public void UpdateUIForSoil(Soil soil)
    {
        Item selectedItem = InventoryManager.instance.GetSelectedItem(false);
        
        if (Player.instance.targetSoil.isEmpty)
        {
            confirmButton.gameObject.SetActive(true);
        }
        else if (Player.instance.targetSoil.currentPlant != null && Player.instance.targetSoil.currentPlant.isWatered)
        {
            confirmButton.gameObject.SetActive(false);
        }
        else if (Player.instance.targetSoil != null && !Player.instance.targetSoil.isEmpty && Player.instance.targetSoil.currentPlant.produce && !Player.instance.targetSoil.currentPlant.waterForFruit)
        {
            confirmButton.gameObject.SetActive(false);
        }
        else if (Player.instance.targetSoil != null && !Player.instance.targetSoil.isEmpty && Player.instance.targetSoil.currentPlant.produce && Player.instance.targetSoil.currentPlant.waterForFruit && Player.instance.targetSoil.currentPlant.CurrentProduceCount >= 1)
        {
            confirmButton.gameObject.SetActive(false);
        }
        else if(Player.instance.targetSoil != null && !Player.instance.targetSoil.isEmpty && !Player.instance.targetSoil.currentPlant.produce)
        {
            confirmButton.gameObject.SetActive(false);
        }
        else if(Player.instance.targetSoil != null && !Player.instance.targetSoil.isEmpty && Player.instance.targetSoil.currentPlant.produce && Player.instance.targetSoil.currentPlant.waterForFruit && Player.instance.targetSoil.currentPlant.CurrentProduceCount == 0)
        {
            confirmButton.gameObject.SetActive(false);
        }
        
        // Check if the soil needs watering and update UI accordingly
        if (Player.instance.targetSoil != null && !Player.instance.targetSoil.isEmpty && selectedItem == waterCan && !Player.instance.targetSoil.currentPlant.produce)
        {
            waterButton.gameObject.SetActive(!Player.instance.targetSoil.currentPlant.isWatered);
        }
        else if (Player.instance.targetSoil != null && !Player.instance.targetSoil.isEmpty && selectedItem == waterCan && Player.instance.targetSoil.currentPlant.produce && Player.instance.targetSoil.currentPlant.waterForFruit) //&& Player.instance.targetSoil.currentPlant.waterForFruit
        {
            waterButton.gameObject.SetActive(!Player.instance.targetSoil.currentPlant.isWatered && Player.instance.targetSoil.currentPlant.CurrentProduceCount == 0);
        }
        else if (!Player.instance.targetSoil.isEmpty)
        {
            waterButton.gameObject.SetActive(false);
        }
    }

    public void HideSeedRequirement()
    {
        // seedText.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        // imagePlant.gameObject.SetActive(false); // Sembunyikan gambar
    }

    private void OnConfirmButtonClicked()
    {
        Player.instance.targetSoil?.OnConfirmButtonClicked();
    }
}
