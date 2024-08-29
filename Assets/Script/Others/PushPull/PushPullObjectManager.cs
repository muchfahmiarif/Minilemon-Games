using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PushPullObjectManager : MonoBehaviour
{
    [SerializeField] private Button pushPullButton;
    [SerializeField] private List<GameObject> pushPullObjects = new List<GameObject>();
    [SerializeField] private float detectionRange = 2.5f; // Jarak maksimum pemain dari objek untuk menampilkan tombol

    [SerializeField] private GameObject player; // Referensi ke objek pemain atau objek yang mewakili pemain

    [SerializeField] private bool isPushPullMode = false; // Status untuk menentukan apakah dalam mode push/pull
    [SerializeField] private bool isInRange = false;

    private void Awake()
    {
        pushPullButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (player != null && !isPushPullMode || isPushPullMode)
        {
            // Periksa jarak antara pemain dan setiap objek dalam pushPullObjects
            foreach (GameObject obj in pushPullObjects)
            {
                if (obj != null) // Pastikan objek masih ada
                {
                    float distanceToPlayer = Vector3.Distance(obj.transform.position, player.transform.position);
                    if (distanceToPlayer <= detectionRange)
                    {
                        isInRange = true;
                        pushPullButton.gameObject.SetActive(true);
                        return;
                    }
                    else
                    {
                        isInRange = false;
                        pushPullButton.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
