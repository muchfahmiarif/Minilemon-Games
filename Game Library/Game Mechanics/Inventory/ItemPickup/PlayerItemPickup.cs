using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemPickup : MonoBehaviour
{
    public float pickupRange = 2f;
    public LayerMask itemLayer;
    private Button buttonPickUp;
    private ItemPickup nearbyItemPickup;

    void Start()
    {
        buttonPickUp = GameObject.FindGameObjectWithTag("PickUp").GetComponent<Button>();
        buttonPickUp.onClick.AddListener(PickUp);
        buttonPickUp.gameObject.SetActive(false); // Initially set the button to be inactive
    }

    void Update()
    {
        CheckForNearbyItems();
    }

    void CheckForNearbyItems()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange, itemLayer);
        if (hitColliders.Length > 0)
        {
            nearbyItemPickup = hitColliders[0].GetComponent<ItemPickup>();
            if (nearbyItemPickup != null)
            {
                buttonPickUp.gameObject.SetActive(true);
                buttonPickUp.interactable = true;
                return;
            }
        }

        buttonPickUp.gameObject.SetActive(false); // Hide the button when no item is nearby
        nearbyItemPickup = null;
    }

    void PickUp()
    {
        if (nearbyItemPickup != null)
        {
            nearbyItemPickup.PickUp();
            buttonPickUp.gameObject.SetActive(false); // Hide the button after picking up the item
        }
    }
}
