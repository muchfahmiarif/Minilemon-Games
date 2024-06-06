using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantManager : MonoBehaviour
{
    public static PlantManager instance;

    public Button waterButton;
    public TMP_Text seedText;
    public Button confirmButton;

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
        seedText.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
    }

    public void UpdateUIForSoil(Soil soil)
    {
        if (soil.isEmpty)
        {
            seedText.text = $"Required Seed: {soil.GetRequiredSeedName()}";
            seedText.gameObject.SetActive(true);
            confirmButton.gameObject.SetActive(true);
        }
        else
        {
            HideSeedRequirement();
        }
    }

    public void HideSeedRequirement()
    {
        seedText.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
    }

    private void OnConfirmButtonClicked()
    {
        Player.instance.targetSoil?.OnConfirmButtonClicked();
    }

    public void UpdateSeedRequirement(string seedName)
    {
        seedText.text = $"Required Seed: {seedName}";
        seedText.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(true);
    }
}
