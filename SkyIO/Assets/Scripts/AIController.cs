using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public ObstacleSpawner obstacleSpawner;

    public float lerpSpeed = 3;
    public float lerpMaxSpeed = 20;

    [Tooltip("Must be between 0 and 100")]
    public float bestDecisionChance = 100;
    [Tooltip("Must be between 0 and 100")]
    public float rightDecisionChance = 100;

    [Tooltip("Must be between 0 and 100 and it is the chance of the AI of moving to one of the center positions when there are no obstacles or boosters/ramps")]
    public float moveToCenterPositionChance = 0;

    public float newTargetReactionDistance = 0;
    public float wrongTargeReactionDistance = 0;

    public float minRightTargetXOffSet = 0;
    public float maxRightTargetXOffSet = 0;

    public float minWrongTargetXOffSet = 0;
    public float maxWrongTargetXOffSet = 0;

    public float bestDecisionMaxDistance = 3;

    int obstacleRowIndex = 0;
    float targetXPosition = 0;
    float targetZPosition = 0;

    enum AIState
    {
        CalculateTargetPosition,
        MovingToTargetPosition,
        MovingToWrongTargetPosition,
        CalculateRightTargetPosition
    }

    AIState aiState = AIState.CalculateTargetPosition;

    List<float[]> obstacleAIValues;

	// Use this for initialization
	void Start () {
        Initialize();
	}


    public void Initialize()
    {
        obstacleAIValues = ObstacleSpawner.rowObstacleAIValues;
    }
	
	// Update is called once per frame
	void Update () {
        switch (aiState)
        {
            case AIState.CalculateTargetPosition:
                if (RightDecision() || !ThereAreBadDecisions(obstacleRowIndex)) // if the AI does the right decision
                {
                    if (ThereAreBestDecisions(obstacleRowIndex) && BestDecision()) // if the AI does the best decision
                    {
                        List<int> bestPositions = BestDecisionPositions(obstacleRowIndex);
                        targetXPosition = ClosestPositionOfRow(obstacleRowIndex, bestPositions);

                        if(Mathf.Abs(targetXPosition - transform.position.x) <= bestDecisionMaxDistance) // if the best decision is within the range we want
                        {
                            targetXPosition += Random.Range(minRightTargetXOffSet, maxRightTargetXOffSet); // little bit of randomness on the pathing
                            targetZPosition = obstacleAIValues[obstacleRowIndex][0];

                            aiState = AIState.MovingToTargetPosition;
                        }
                        else
                        {
                            List<int> rightPositions = RightDecisionPositions(obstacleRowIndex);
                            targetXPosition = ClosestPositionOfRow(obstacleRowIndex, rightPositions);
                            targetXPosition += Random.Range(minRightTargetXOffSet, maxRightTargetXOffSet); // little bit of randomness on the pathing

                            targetZPosition = obstacleAIValues[obstacleRowIndex][0];

                            aiState = AIState.MovingToTargetPosition;
                        }

                    }
                    else // if the AI doesn't do the best decision
                    {
                        if (!ThereAreBadDecisions(obstacleRowIndex) && !ThereAreBestDecisions(obstacleRowIndex) && Random.Range(1,101) <= moveToCenterPositionChance) // if no obstacles and no boosts or ramps, AI will prefer to move into one of the center positions
                        {
                            targetXPosition = (ObstacleSpawner.obstacleSpaceLength / 2) + (ObstacleSpawner.obstacleSpaceLength * Random.Range(1,3)); // random between the two center positions

                            targetXPosition += Random.Range(minRightTargetXOffSet, maxRightTargetXOffSet); // little bit of randomness on the pathing

                            aiState = AIState.MovingToTargetPosition;
                        }
                        else
                        {
                            List<int> rightPositions = RightDecisionPositions(obstacleRowIndex);
                            targetXPosition = ClosestPositionOfRow(obstacleRowIndex, rightPositions);
                            targetXPosition += Random.Range(minRightTargetXOffSet, maxRightTargetXOffSet); // little bit of randomness on the pathing

                            targetZPosition = obstacleAIValues[obstacleRowIndex][0];

                            aiState = AIState.MovingToTargetPosition;
                        }
                    }
                }
                else // if the AI doesn't do the right decision
                {
                    List<int> badPositions = BadDecisionPositions(obstacleRowIndex);
                    targetXPosition = ClosestPositionOfRow(obstacleRowIndex, badPositions);
                    targetXPosition += Random.Range(minWrongTargetXOffSet, maxWrongTargetXOffSet);
                    targetZPosition = obstacleAIValues[obstacleRowIndex][0];

                    aiState = AIState.MovingToWrongTargetPosition;
                }

                break;

            case AIState.MovingToTargetPosition:

                float lerpedX = Mathf.Lerp(transform.position.x, targetXPosition, lerpSpeed * Time.deltaTime);
                lerpedX = Mathf.Clamp(lerpedX, transform.position.x - (lerpMaxSpeed * Time.deltaTime), transform.position.x + (lerpMaxSpeed * Time.deltaTime)); // clamps it so we dont exceed the maximum speed
                transform.position = new Vector3(lerpedX, transform.position.y, transform.position.z);


                if (transform.position.z >= targetZPosition + newTargetReactionDistance)
                {
                    obstacleRowIndex++;
                    aiState = AIState.CalculateTargetPosition;
                }

                break;

            case AIState.MovingToWrongTargetPosition:

                lerpedX = Mathf.Lerp(transform.position.x, targetXPosition, lerpSpeed * Time.deltaTime);
                lerpedX = Mathf.Clamp(lerpedX, transform.position.x - (lerpMaxSpeed * Time.deltaTime), transform.position.x + (lerpMaxSpeed * Time.deltaTime)); // clamps it so we dont exceed the maximum speed
                transform.position = new Vector3( lerpedX, transform.position.y, transform.position.z);

                if (transform.position.z >= targetZPosition - wrongTargeReactionDistance)
                {
                    aiState = AIState.CalculateRightTargetPosition;
                }
                break;

            case AIState.CalculateRightTargetPosition:
                List<int> rightTargetPositions = RightDecisionPositions(obstacleRowIndex);
                targetXPosition = ClosestPositionOfRow(obstacleRowIndex, rightTargetPositions);
                targetXPosition += Random.Range(minRightTargetXOffSet, maxRightTargetXOffSet); // little bit of randomness on the pathing
                targetZPosition = obstacleAIValues[obstacleRowIndex][0];

                aiState = AIState.MovingToTargetPosition;
                break;
        }
	}

    float ClosestPositionOfRow(int row, List<int> indexes) // returns X position of the closest space in the row to the AI
    {
        float zPos = obstacleAIValues[row][0];
        float xPos = obstacleSpawner.minWidth;

        float distance = float.MaxValue;
    
        float savedXPos = 0;
        foreach(int index in indexes)
        {
            xPos = (ObstacleSpawner.obstacleSpaceLength / 2) + (ObstacleSpawner.obstacleSpaceLength * (index - 1));

            float newDistance = Vector2.Distance(new Vector2(xPos,zPos), new Vector2(transform.position.x,transform.position.z)); // distance between this AI and the center of the space index represents
            if (newDistance < distance)
            {
                savedXPos = xPos;
                distance = newDistance;
            }
        }

        return savedXPos;
    }

    List<int> BadDecisionPositions(int row)
    {
        List<int> badPositions = new List<int>();

        for(int i = 1; i < obstacleAIValues[0].Length; i++)
        {
            if( obstacleAIValues[row][i] < 0)
            {
                badPositions.Add(i);
            }
        }

        return badPositions;
    }

    bool ThereAreBadDecisions(int row)
    {
        for(int i = 1; i < obstacleAIValues[0].Length; i++)
        {
            if(obstacleAIValues[row][i] < 0)
            {
                return true;
            }
        }

        return false;
    }

    List<int> BestDecisionPositions(int row)
    {
        List<int> bestPositions = new List<int>();

        for (int i = 1; i < obstacleAIValues[0].Length; i++)
        {
            if (obstacleAIValues[row][i] >= 1)
            {
                bestPositions.Add(i);
            }
        }

        return bestPositions;
    }

    bool ThereAreBestDecisions(int row)
    {
        for(int i = 1; i < obstacleAIValues[0].Length; i++)
        {
            if(obstacleAIValues[row][i] > 0)
            {
                return true;
            }
        }

        return false;
    }

    bool BestDecision()
    {

        if(Random.Range(1,101) <= bestDecisionChance)
        {
            return true;
        }


        return false;
    }

    List<int> RightDecisionPositions(int row)
    {
        List<int> rightPositions = new List<int>();
        for(int i = 1; i < obstacleAIValues[0].Length; i++)
        {
            if(obstacleAIValues[row][i] >= 0)
            {
                rightPositions.Add(i);
            }
        }

        return rightPositions;
    }


    bool RightDecision()
    {

        if (Random.Range(1,101) <= rightDecisionChance)
        {
            return true;
        }

        return false;
    }
}
