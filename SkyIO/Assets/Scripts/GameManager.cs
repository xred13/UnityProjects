using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject aiPrefab;
    public Transform[] aiSpawnPositions;
    public int aiAmmount = 5;
    public Transform aiParent;
    public TerrainGenerator terrainGenerator;
    public ObstacleSpawner obstacleSpawner;

    public Transform[] players;
    public int[] positions;
    public int firstPlaceScore;
    public int defaultScore;

    public float endRaceZ;

    int position = 1;

    void InitializePlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("Games Won")) // settup prefs only if we haven't yet
        {
            PlayerPrefs.SetInt("Games Won", 0);
            PlayerPrefs.SetInt("Score", 0);
        }
    }

    void SpawnAI()
    {
        for(int i = 0; i < aiAmmount; i++)
        {
            GameObject ai = Instantiate(aiPrefab);
            ai.transform.position = aiSpawnPositions[i].position;
            ai.GetComponent<AIController>().obstacleSpawner = obstacleSpawner;
            ai.GetComponent<BallMovement>().terrainGenerator = terrainGenerator;
            ai.transform.parent = aiParent;

            if (PlayerPrefs.GetInt("Games Won") <= 2)
            {
                ai.GetComponent<AIController>().lerpSpeed = 5 - Random.Range(1f, 2f);
            }
            else if (PlayerPrefs.GetInt("Games Won") <= 5)
            {
                ai.GetComponent<AIController>().lerpSpeed = 7 - Random.Range(1f, 2f);
            }
            else if (PlayerPrefs.GetInt("Games Won") <= 10)
            {
                ai.GetComponent<AIController>().lerpSpeed = 10 - Random.Range(1f, 2f);
            }
            else if (PlayerPrefs.GetInt("Games Won") <= 20)
            {
                ai.GetComponent<AIController>().lerpSpeed = 15 - Random.Range(1f, 2f);
            }
            else
            {
                ai.GetComponent<AIController>().lerpSpeed = 20 - Random.Range(1f, 2f);
            }
        }
    }

    private void Awake()
    {
        InitializePlayerPrefs();

        terrainGenerator.Initialization(); // initialize terrain, obstacles, lights

        SpawnAI();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
            InitializePlayerPrefs();
        }

        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].position.z >= endRaceZ && positions[i] == 0)
            {
                positions[i] = position++;
                if(players[i].gameObject.GetComponent<PlayerInput>() != null)// if it is a real player and not the AI
                {
                    switch (positions[i])
                    {
                        case 1:
                            PlayerPrefs.SetInt("Games Won", PlayerPrefs.GetInt("Games Won") + 1);
                            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + firstPlaceScore);
                            break;
                        default:
                            PlayerPrefs.SetInt("Score", PlayerPrefs.GetInt("Score") + defaultScore - (int)(defaultScore * positions[i] * 0.1));
                            break;
                    }
                }
            }
        }
    }


}
