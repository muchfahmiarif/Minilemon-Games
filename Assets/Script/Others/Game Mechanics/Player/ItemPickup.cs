using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    public GameObject parent;
    public bool isInInventory = false;

    public void PickUp()
    {
        bool wasPickedUp = InventoryManager.instance.AddItem(item);
        if (wasPickedUp)
        {
            isInInventory = true; // Setelah diambil, tandai item sebagai isInInventory
            Destroy(parent);
        }
    }
}
