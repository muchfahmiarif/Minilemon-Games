using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    public Transform handTransform; 

    private GameObject currentItem;
    public Soil targetSoil; 

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

    private void Update()
    {
        if (currentItem != null && InventoryManager.instance.IsSelectedSlotEmpty())
        {
            DropItem();
        }

        LockNearestSoil();
    }

    public void HoldItem(Item item)
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
        }

        // Instantiate the item and set it as a child of the hand transform
        currentItem = Instantiate(item.prefab, handTransform);

        // Apply the position, rotation, and scale from the item settings
        currentItem.transform.localPosition = item.holdPosition;
        currentItem.transform.localRotation = Quaternion.Euler(item.holdRotation);
        currentItem.transform.localScale = item.holdScale;
    }

    public void DropItem()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }
    }

    public void WaterPlant()
    {
        if (targetSoil != null && targetSoil.currentPlant != null)
        {
            Debug.Log("Found soil: " + targetSoil.name);
            targetSoil.currentPlant.WaterPlant();
            targetSoil.MakeSoilWatered();
        }
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
}
