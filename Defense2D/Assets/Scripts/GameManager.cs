using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public CanvasManager canvas;

    [Space]

    public float afterShootingCircleDistanceTravelToCenter;
    public float playerInnerCircleRadius;

    [Space]

    public Transform circlesParent;
    public GameObject circle;

    public GameObject circleHealthText;

    [HideInInspector]
    public List<GameObject> circles;

    public float circleMaximumSize;
    public float circleMinimumSize = 1f;
    public float circleSizeTimesHealth = 0.1f;


    public int firstStageCirclesAmmount;
    public int circlesAmmountIncrement;

    [Header("Size must be equal to the variable \"numberOfSteps\" + 1, each index corresponds to a step, from inner steps to outwards, ex: first value is the number of circles on the first step and so forth")]
    public int[] firstStageStepsCircleAmmount;
    public int[] stepsCircleAmmountIncrement;
    public int[] stepsCircleAmmountStagesToIncrement;
    public int[] maximumStepsCircleAmmount;
    [HideInInspector]
    public int[] stepsCircleAmmountStageLastIncrement;

    public int[] firstStageStepsCircleMinimumHealth;
    public int[] firstStageStepsCircleMaximumHealth;
    public int[] stepsCircleHealthIncrement;
    public int[] stepsCircleHealthStagesToIncrement;
    [HideInInspector]
    public int[] stepsCircleHealthStageLastIncrement;


    [Space]

    public CircleSpawning circleSpawning;

    [Space]

    public float rotateAroundMinimumDegrees;
    public float additionalCircleSpawnDistance;

    [Space]

    public float maximumStepDistance;
    public float minimumStepDistance;
    public int numberOfSteps;

    [HideInInspector]
    public float stepSize;

    [HideInInspector]
    public int stage = 1;

    int circlesAlive = 0;
    int circlesThatHaveSpawned = 0;

    public enum GameState
    {
        InitializeStage,
        OnGoingWaitShooting,
        OnGoingDuringShooting,
        OnGoingAfterShooting,
        EndStage
    }

    public GameState gameState = GameState.InitializeStage;

    // Start is called before the first frame update
    void Start()
    {
        circles = new List<GameObject>();
        stepSize = (maximumStepDistance - minimumStepDistance) / (numberOfSteps - 1);

        stepsCircleAmmountStageLastIncrement = new int[numberOfSteps+1];
        stepsCircleHealthStageLastIncrement = new int[numberOfSteps+1];
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameState.InitializeStage:

                bool spawned = true;
                for(int i = 0; i < numberOfSteps; i++)
                {
                    for(int k = 0; k < firstStageStepsCircleAmmount[i]; k++)
                    {
                        spawned = SpawnCircle(i);

                        if (!spawned)
                        {
                            break;
                        }

                    }
                }

                gameState = GameState.OnGoingWaitShooting;
                break;

            case GameState.OnGoingWaitShooting:
                // we change the state on Player.cs when the player shoots
                break;

            case GameState.OnGoingDuringShooting:
                // we change the state on bullet manager
                break;

            case GameState.OnGoingAfterShooting:
                if(circles.Count > 0)
                {
                    MoveCirclesToCenter();
                }

                if (circlesThatHaveSpawned < firstStageCirclesAmmount) // spawn new circle if there are still circles left to spawn in this stage
                {
                    SpawnCircle(numberOfSteps - 1);

                    gameState = GameState.OnGoingWaitShooting;
                }
                else
                {
                    if (circlesAlive == 0)
                    {
                        gameState = GameState.EndStage;
                    }
                    else
                    {
                        gameState = GameState.OnGoingWaitShooting;
                    }
                }

                break;


            case GameState.EndStage:

                firstStageCirclesAmmount += circlesAmmountIncrement;
                
                for(int i = 0; i < firstStageStepsCircleAmmount.Length; i++)
                {
                    if(stepsCircleAmmountStageLastIncrement[i] + stepsCircleAmmountStagesToIncrement[i] == stage)
                    {
                        stepsCircleAmmountStagesToIncrement[i] += stepsCircleAmmountStagesToIncrement[i];
                        firstStageStepsCircleAmmount[i] += stepsCircleAmmountIncrement[i];
                    }

                    if (stepsCircleHealthStageLastIncrement[i] + stepsCircleHealthStagesToIncrement[i] == stage)
                    {
                        stepsCircleHealthStagesToIncrement[i] += stepsCircleHealthStagesToIncrement[i];
                        firstStageStepsCircleMinimumHealth[i] += stepsCircleHealthIncrement[i];
                        firstStageStepsCircleMaximumHealth[i] += stepsCircleHealthIncrement[i];
                    }
                }

                stage += 1;

                gameState = GameState.InitializeStage;
                break;
        }
    }

    bool SpawnCircle(int step)
    {
        GameObject circleSpawned = Instantiate(circle);
        circleSpawned.transform.parent = circlesParent;
        Circle circleSpawnedComponent = circleSpawned.GetComponent<Circle>();

        circleSpawnedComponent.gameManager = this;
        circleSpawnedComponent.SetHealth(Random.Range(firstStageStepsCircleMinimumHealth[step], firstStageStepsCircleMaximumHealth[step] + 1));

        circleSpawnedComponent.index = circles.Count;
        circles.Add(circleSpawned);

        circleSpawning.MoveSpawnedCircleToPosition(circleSpawned, step, out bool spawned);

        // add a health text to the circle 
        GameObject spawnedCircleHealthText = Instantiate(circleHealthText);
        spawnedCircleHealthText.transform.SetParent(canvas.transform);
        spawnedCircleHealthText.transform.position = canvas.WorldToCanvasPoint(circleSpawned.transform.position);
        circleSpawnedComponent.healthText = spawnedCircleHealthText.GetComponent<TextMeshProUGUI>();

        spawnedCircleHealthText.GetComponent<TextMeshProUGUI>().text = circleSpawnedComponent.health.ToString();

        if (spawned)
        {
            circlesAlive++;
            circlesThatHaveSpawned++;
        }

        return spawned;
    }

    void MoveCirclesToCenter()
    {

        for(int i = 0; i < circles.Count; i++)
        {
            Transform circleTransform = circles[i].transform;
            Vector3 moveDirection = new Vector3(-circleTransform.position.x, -circleTransform.position.y, 0);

            circleTransform.position += moveDirection.normalized * afterShootingCircleDistanceTravelToCenter;

            Circle circleComponent = circleTransform.GetComponent<Circle>();
            circleComponent.healthText.transform.position = canvas.WorldToCanvasPoint(circleTransform.position);

            if (circleComponent.OnPlayerInnerCircle())
            {
                DestroyCircle(circleComponent.index);
            }
        }
    }

    public void DestroyCircle(int index)
    {
        circlesAlive--;

        Destroy(circles[index].GetComponent<Circle>().healthText);
        Destroy(circles[index]);
        circles.RemoveAt(index);

        UpdateIndexes(index);
    }

    void UpdateIndexes(int index)
    {
        for(int i = index; i < circles.Count; i++)
        {
            circles[i].GetComponent<Circle>().index = i;
        }

    }
}
