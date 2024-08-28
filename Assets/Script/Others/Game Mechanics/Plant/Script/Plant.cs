using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] public string PlantName;
    [SerializeField] GameObject seedModel;
    [SerializeField] GameObject youngPlantModel;
    [SerializeField] public GameObject maturePlantModel;
    [SerializeField] public Sprite plantImage; // Tambahkan referensi gambar untuk tanaman

    [SerializeField] List<GameObject> plantProduceSpawns;
    [SerializeField] GameObject producePrefab;

    public float secondsOfPlanting; // Seconds since "birth"
    [SerializeField] float plantAgeInSeconds = 0; // depends on the watering frequency

    [SerializeField] float secondsForYoungModel; // Seconds for the plant to reach the young stage
    [SerializeField] float secondsForMatureModel; // Seconds for the plant to reach the mature stage
    [SerializeField] float secondsForFirstProduceBatch; // Seconds for the plant to produce its first batch of fruits

    [SerializeField] float secondsForNewProduce; // Seconds it takes for new fruit to grow after the initial batch
    [SerializeField] float secondsRemainingForNewProduceCounter;

    [SerializeField] public bool isOneTimeHarvest;
    [SerializeField] public bool isOneTimeHarvestWithManys;
    [SerializeField] public bool isHasfruitedonce = false;

    [SerializeField] public bool isWatered = false; // Only if the plant is watered, it will "age"
    [SerializeField] bool isWateredThisStage; // Indicates whether the plant has been watered in this stage
    bool youngStage = true; //
    bool matureStage = true; // 
    public bool produce = false;
    public bool waterForFruit = false;

    public int maxProduceCount; // Maximum number of produce that can be produced
    private Soil soil; // reference from soil code

    public int CurrentProduceCount; //{ get; private set; } // Tambahkan properti untuk currentProduceCount
    Animator animator;


    private void Start()
    {
        secondsOfPlanting = Time.time;
        maxProduceCount = plantProduceSpawns.Count;
        animator = GetComponent<Animator>();
        seedModel.SetActive(false);
        youngPlantModel.SetActive(false);
        maturePlantModel.SetActive(false);
    }

    private void Update()
    {
        if (isWatered)
        {
            plantAgeInSeconds += Time.deltaTime;
        }
        else if (!isWatered)
        {
            GetComponentInParent<Soil>().MakeSoilNotWatered();
        }
        seedModel.SetActive(plantAgeInSeconds < secondsForYoungModel);

        // CheckGrowth();
        CheckGrowthAnimations();

        if (!isOneTimeHarvest)
        {
            if (produce)
            {
                int currentProduceCount = 0;
                CheckProduce();

                foreach (GameObject spawn in plantProduceSpawns)
                {
                    if (spawn.transform.childCount > 0)
                    {
                        currentProduceCount++;
                    }
                }

                CurrentProduceCount = currentProduceCount; // Perbarui nilai properti

                if (CurrentProduceCount < maxProduceCount)
                {
                    waterForFruit = true;
                }
                else
                {
                    waterForFruit = false;
                }
            }
        }


        if (isOneTimeHarvest && isOneTimeHarvestWithManys)
        {
            if (produce)
            {
                int currentProduceCount = 0;
                CheckProduce();

                foreach (GameObject spawn in plantProduceSpawns)
                {
                    if (spawn.transform.childCount > 0)
                    {
                        currentProduceCount++;
                    }
                }

                CurrentProduceCount = currentProduceCount; // Perbarui nilai properti

                if (CurrentProduceCount < maxProduceCount)
                {
                    waterForFruit = true;
                }
                else
                {
                    waterForFruit = false;
                }
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

    private void CheckGrowthAnimations()
    {
        if (plantAgeInSeconds >= secondsForYoungModel && plantAgeInSeconds < secondsForMatureModel && youngStage)
        {
            PlayYoungModelAnimation();
            youngPlantModel.SetActive(plantAgeInSeconds >= secondsForYoungModel && plantAgeInSeconds < secondsForMatureModel);
            isWatered = false;
            isWateredThisStage = false;
            youngStage = false;
        }
        else if (plantAgeInSeconds >= secondsForMatureModel && matureStage && !produce)
        {
            PlayMatureModelAnimation();
            maturePlantModel.SetActive(plantAgeInSeconds >= secondsForMatureModel);
            isWatered = false;
            isWateredThisStage = false;
            matureStage = false;
            produce = true;
        }
        GetComponentInParent<Soil>().UpdateCanvasPosition();
    }

    private void PlayYoungModelAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("YoungModel");
            // Debug.Log("animasi young ");
        }

    }

    private void PlayMatureModelAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("MatureModel");
        }
    }

    private void MakePlantPickable()
    {
        // Implement logic to make the plant pickable
    }

    private void CheckProduce()
    {
        if (plantAgeInSeconds >= secondsForFirstProduceBatch && isWatered)
        {
            if (CurrentProduceCount < maxProduceCount)
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
        isHasfruitedonce = true;
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

    public Sprite GetPlantImage()
    {
        return plantImage;
    }

    public float GetHeightForCurrentStage()
{
    if (GetComponentInParent<Soil>().currentPlantObject != null)
    {
        if (plantAgeInSeconds < secondsForYoungModel)
        {
            return Mathf.Max(seedModel.GetComponent<Renderer>().bounds.size.y, 1f);
        }
        else if (plantAgeInSeconds >= secondsForYoungModel && plantAgeInSeconds < secondsForMatureModel)
        {
            return Mathf.Max(youngPlantModel.GetComponent<Renderer>().bounds.size.y + 0.35f, 1f);
        }
        else
        {
            return Mathf.Max(maturePlantModel.GetComponent<Renderer>().bounds.size.y + 0.35f, 1f);
        }
    }

    return 1f; // or any other default value you deem appropriate
}


}
