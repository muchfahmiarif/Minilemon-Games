// Plant.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] GameObject seedModel;
    [SerializeField] GameObject youngPlantModel;
    [SerializeField] GameObject maturePlantModel;

    [SerializeField] List<GameObject> plantProduceSpawns;
    [SerializeField] GameObject producePrefab;

    public float secondsOfPlanting; // Seconds since "birth"
    [SerializeField] float plantAgeInSeconds = 0; // depends on the watering frequency

    [SerializeField] float secondsForYoungModel; // Seconds for the plant to reach the young stage
    [SerializeField] float secondsForMatureModel; // Seconds for the plant to reach the mature stage
    [SerializeField] float secondsForFirstProduceBatch; // Seconds for the plant to produce its first batch of fruits

    [SerializeField] float secondsForNewProduce; // Seconds it takes for new fruit to grow after the initial batch
    [SerializeField] float secondsRemainingForNewProduceCounter;

    [SerializeField] bool isOneTimeHarvest;
    [SerializeField] public bool isWatered = false; // Only if the plant is watered, it will "age"
    [SerializeField] bool isWateredThisStage; // Indicates whether the plant has been watered in this stage
    bool youngStage = true; //
    bool matureStage = true; // 
    bool produce = false;


    private int maxProduceCount; // Maximum number of produce that can be produced
    private Soil soil; // reference from soil code

    private void Start()
    {
        secondsOfPlanting = Time.time;
        maxProduceCount = plantProduceSpawns.Count;
    }

    private void Update()
    {
        if (isWatered)
        {
            plantAgeInSeconds += Time.deltaTime;
        }

        CheckGrowth();

        if (!isOneTimeHarvest)
        {
            if (produce)
            {
                CheckProduce();
            }
        }

        // Reset isWateredThisStage for the next stage
        if (plantAgeInSeconds >= secondsForYoungModel && plantAgeInSeconds < secondsForMatureModel & youngStage)
        {
            isWatered = false;
            isWateredThisStage = false;
            youngStage = false;
        }
        else if (plantAgeInSeconds >= secondsForMatureModel && matureStage && !produce)
        {
            isWatered = false;
            isWateredThisStage = false;
            matureStage = false;
            produce = true;
        }
    }

    private void CheckGrowth()
    {
        seedModel.SetActive(plantAgeInSeconds < secondsForYoungModel);
        youngPlantModel.SetActive(plantAgeInSeconds >= secondsForYoungModel && plantAgeInSeconds <= secondsForMatureModel);
        maturePlantModel.SetActive(plantAgeInSeconds >= secondsForMatureModel);

        if (plantAgeInSeconds >= secondsForMatureModel && isOneTimeHarvest)
        {
            MakePlantPickable();
        }
    }

    private void MakePlantPickable()
    {
        // Implement logic to make the plant pickable
    }

    private void OnDestroy()
    {
        GetComponentInParent<Soil>().isEmpty = true;
        // GetComponentInParent<Soil>().plantName = "";
        GetComponentInParent<Soil>().currentPlant = null;
    }

    private void CheckProduce()
    {
        if (plantAgeInSeconds >= secondsForFirstProduceBatch && isWateredThisStage && produce)
        {
            int currentProduceCount = 0;

            foreach (GameObject spawn in plantProduceSpawns)
            {
                if (spawn.transform.childCount > 0)
                {
                    currentProduceCount++;
                }
            }

            if (currentProduceCount < maxProduceCount)
            {
                if (secondsRemainingForNewProduceCounter <= 0)
                {
                    GenerateProduceForEmptySpawn();
                    secondsRemainingForNewProduceCounter = secondsForNewProduce;
                }
                else
                {
                    secondsRemainingForNewProduceCounter -= Time.deltaTime;
                }
            }
        }

    }

    private void GenerateProduceForEmptySpawn()
    {
        foreach (GameObject spawn in plantProduceSpawns)
        {
            if (spawn.transform.childCount == 0)
            {
                // Instantiate the produce from prefabs
                GameObject produce = Instantiate(producePrefab);

                // Set the produce to be a child of the current spawn in the list
                produce.transform.parent = spawn.transform;

                // position the produce in the middle of the spawn
                Vector3 producePosition = Vector3.zero;
                producePosition.y = 0f;
                produce.transform.localPosition = producePosition;
            }
        }
        isWatered = false;
        isWateredThisStage = false;
    }

    public void WaterPlant()
    {
        if (!isWatered)
        {
            isWateredThisStage = true;
            isWatered = true;
        }
    }
}
