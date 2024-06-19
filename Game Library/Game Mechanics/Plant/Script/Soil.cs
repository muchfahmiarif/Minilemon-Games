using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour
{
    public bool isEmpty = true;
    public bool playerInRange;
    public bool isLocked = false;

    [SerializeField] private Item requiredSeed;
    [SerializeField] private string plantPrefabName;
    [SerializeField] public Material defaultMaterial;
    [SerializeField] public Material wateredMaterial;

    public Plant currentPlant;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (!isLocked)
        {
            float distance = Vector3.Distance(Player.instance.transform.position, transform.position);

            if (distance < 10f)
            {
                playerInRange = true;
                ShowSeedRequirement();
            }
            else
            {
                playerInRange = false;
                HideSeedRequirement();
            }
        }
    }

    private void ShowSeedRequirement()
    {
        if (isEmpty)
        {
            PlantManager.instance.UpdateSeedRequirement(requiredSeed.name);
        }
        else
        {
            PlantManager.instance.HideSeedRequirement();
        }
    }

    private void HideSeedRequirement()
    {
        PlantManager.instance.HideSeedRequirement();
    }

    internal void OnConfirmButtonClicked()
    {
        if (isLocked)
        {
            Debug.Log("Confirm button clicked. Checking inventory for required seed.");

            Item selectedItem = InventoryManager.instance.GetSelectedItem(false);

            if (selectedItem != null)
            {
                Debug.Log("Selected item: " + selectedItem);
            }
            else
            {
                Debug.Log("No item selected from inventory.");
            }

            if (selectedItem == requiredSeed && isEmpty)
            {
                Debug.Log("Required seed found in inventory and soil is empty. Planting seed.");
                PlantSeed();
            }
            else
            {
                if (selectedItem != requiredSeed)
                {
                    Debug.Log("Selected item is not the required seed.    selectedItem:" + selectedItem + "      requiredSeed:" + requiredSeed);
                }
                if (!isEmpty)
                {
                    Debug.Log("Soil is not empty.");
                }
            }
        }
    }

    internal void PlantSeed()
    {
        isEmpty = false;

        GameObject instantiatedPlant = Instantiate(Resources.Load(plantPrefabName) as GameObject, transform);

        Vector3 plantPosition = Vector3.zero;
        plantPosition.y = 0f;
        instantiatedPlant.transform.localPosition = plantPosition;

        InventoryManager.instance.GetSelectedItem(true);

        currentPlant = instantiatedPlant.GetComponent<Plant>();
        currentPlant.secondsOfPlanting = Time.time;

        HideSeedRequirement();
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
}
