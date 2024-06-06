using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // The item to be picked up
    public GameObject parent; // The parent GameObject to destroy upon pickup

    public void PickUp()
    {
        bool wasPickedUp = InventoryManager.instance.AddItem(item); // Add the item to the inventory
        if (wasPickedUp)
        {
            Destroy(parent); // Destroy the parent GameObject if the item was picked up
        }
    }
}
