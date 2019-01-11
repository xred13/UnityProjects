using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GameManagerV2 : MonoBehaviour {

    public TextMeshProUGUI scoreText, streaktext;

    public TerrainGenerator terrainGenerator;
    public GameObject player;
    [Tooltip("Y value after which the player receives 1 score and enables streaks.")]
    public float scoringHeight;
    [Tooltip("Percentage of the wave where the player can fall on and increment streak. Starts counting from the beginning of the downhill.")]
    public float percentageOfWaveToStreak;
    [Tooltip("Percentage of the wave where the player can fall on and die. Starts counting from the end of the uphill (still not implemented).")] 
    public float percentageOfWaveToDie;

    int streak = 1;
    int score = 0;
    bool canScore = true;
    bool onStreak = false;
    bool canStreak = false;
    bool canDie = false;

    bool isUphill;
    [HideInInspector]
    public float upHillMaxPos, downHillMaxPos; // sets the reference to first plane's vertice position

    // Use this for initialization
    void Start () {
        player.GetComponent<PlayerManagerV2>().OnLanding += OnLanding;

        upHillMaxPos = terrainGenerator.waveSize / 2;
        downHillMaxPos = terrainGenerator.waveSize;
        
	}
	
	// Update is called once per frame
	void LateUpdate () {

        if(player.transform.position.z > downHillMaxPos) // setting the right values for uphillmaxpos and downhillmaxpos floats (reference for the positions of the current wave uphill and downhill's)
        {
            upHillMaxPos += terrainGenerator.waveSize / 2;
            downHillMaxPos += terrainGenerator.waveSize / 2;
        }

        if(player.transform.position.z <= upHillMaxPos || player.transform.position.z == downHillMaxPos)
        {
            isUphill = true;
        }
        else
        {
            isUphill = false;
        }


        #region ScoringAboveHeightLine
        if(player.transform.position.y >= scoringHeight)
        {
            if (canScore && !onStreak)
            {
                score += 1;
                Debug.Log("Scored once for going above the height line!");
                canScore = false;
                canStreak = true;
                canDie = true;
                UpdateScoreAndStreak();
            }
        }
        else
        {
            canDie = false;
            canScore = true;
        }

        #endregion





    }

    void OnLanding(float z)
    {
        if (player.GetComponent<PlayerManagerV2>().weCanCheck)
        {
            if (!isUphill && canStreak)
            {
                if (z <= upHillMaxPos + percentageOfWaveToStreak * (terrainGenerator.waveSize / 2) * 0.01)
                {
                    streak *= 2;
                    canStreak = true;
                    onStreak = true;
                    score += streak;
                }
                else
                {
                    streak = 1;
                    onStreak = false;
                    canStreak = false;
                }
            }
            else
            {
                if (canDie)
                {
                    //check if he dies

                }
                streak = 1;
                onStreak = false;
                canStreak = false;
            }
        }


        UpdateScoreAndStreak();

    }

    void UpdateScoreAndStreak()
    {
        scoreText.text = score.ToString();
        streaktext.text = streak.ToString() + "x";
    }
}
