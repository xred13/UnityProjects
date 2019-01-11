using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObstacleSpawner : MonoBehaviour
{
    public GameObject forestCollider;

    [Tooltip("Our obstacle's parent.")]
    public Transform obstaclesParent; // the parent of our obstacles
    public LayerMask groundLayer;//Which layer is ground. Used when finding height. Else, obstacles might spawn on top of eachother

    [Tooltip("Must have same length as obstacles GameObject array. Usage example: setting index 0 of this array to 50 and considering obstacles[0] is a ramp, there will be no ramps 50 units after a ramp spawns.")]
    public float[] minDistanceBetweenSameObstacles;
    float[] lastPositionPlaced; // updated every time we spawn an obstacle, has same length as obstacles array and we use obstacles exact index to update it (example: we spawn ramp which is obstacles[0], we update lastPositionPlaced[0] = ramp.transform.position

    [Tooltip("Must have same length as obstacles GameObject array. Usage example: setting index 0 of this array to 1 makes it as so that obstacles[0] can only be spawned once on each row of obstacles.")]
    public int[] maxRowAmmount;
    int[] timesPlacedInRow; // used for reference, every time we are spawning a row of obstacles, if we spawn an obstacle we increment placedXTimes[obstacleIndex] 

    public GameObject[] obstacles;

    [Tooltip("Ammount of spaces in a row we have for obstacles to spawn.")]
    public int obstacleSpacesInARow;
    public static int obstacleSpacesInRow;

    public float minDistanceBetweenRows;
    public float maxDistanceBetweenRows;

    public int minObstaclesInRow;
    public int maxObstaclesInRow;

    [Tooltip("The minimum x value of our rows.")]
    public float minWidth;
    [Tooltip("The maximum x value of our rows.")]
    public float maxWidth;

    [Tooltip("Position where we start spawning rows of obstacles.")]
    public float startPosition;
    [Tooltip("Endless mode or not")]
    public bool isEndless;
    [Tooltip("If we are not in endless mode, the game will not spawn obstacles after this Z value.")]
    public float endPosition;

    [Tooltip("The exact Z value of the meshes we use for terrain.")]
    public float meshZSize;


    float width; // maxWidth - minWidth
    float obstacleSpaceInRowWidth; // the width of a space in the row
    float rowPosition; // incremented every time we spawn a row

    float[] rowSpacesCenterPosition; // length is equal to the number of spaces we have in a row, each index correspondes to a space, from left to right and the value correspondes to the X position of that space

    public float obstacleSpaceLengthValue;
    public static float obstacleSpaceLength;
    [HideInInspector]
    public static List<float[]> rowObstacleAIValues; // each array is a row of obstacles, first value is the z position, the next ones are the obstacle values for the AI: ex: ramp is 1, nothing is 0, tree is -1



    [Header("Random Forest Generation")]
    public GameObject[] trees;
    public float beforeSpace, afterSpace, spaceBetweenRows; // space means length
    public int chance;

    public int randomForestRows, randomForestColumns, randomForestChance;
    public float randomForestMinTreeWidthDistance, randomForestMaxTreeWidthDistance, randomForestMinTreeLengthDistance, randomForestMaxTreeLengthDistance, randomForestMinWidthLeft, randomForestMaxWidthLeft, randomForestMinWidthRight, randomForestMaxWidthRight;
    [HideInInspector]
    public float randomForestTotalLength;

    float currentRowPosition;
    float spaceWidth;


    GameObject RandomTree()
    {
        return trees[Random.Range(0, trees.Length)];
    }

    Vector3[][] GenerateDoubleArray(float rowPosition, bool isLeft)
    {
        currentRowPosition = rowPosition;
        currentRowPosition += beforeSpace;

        Vector3[][] treesArray = new Vector3[randomForestRows][];
        for (int i = 0; i < treesArray.Length; i++) // init arrays for the trees positions
        {
            treesArray[i] = new Vector3[randomForestColumns];
        }

        for (int i = 0; i < treesArray.Length; i++) // rows
        {
            float xPos = 0;
            float maximumWidth = 0;
            if (isLeft)
            {
                xPos = randomForestMinWidthLeft;
                maximumWidth = randomForestMaxWidthLeft;
            }
            else
            {
                xPos = randomForestMinWidthRight;
                maximumWidth = randomForestMaxWidthRight;
            }
            for (int k = 0; k < treesArray[i].Length; k++) // columns
            {
                if (xPos < maximumWidth && Random.Range(0, randomForestChance) == 0) // if width within limits[minwidth,maxwidth] and chance, we have a new position here for a tree
                {
                    treesArray[i][k] = new Vector3(xPos, FindHeight(new Vector3(xPos, 20, currentRowPosition)), currentRowPosition);
                }

                xPos += Random.Range(randomForestMinTreeWidthDistance, randomForestMaxTreeWidthDistance); // increment width

            }
            currentRowPosition += Random.Range(randomForestMinTreeLengthDistance, randomForestMaxTreeLengthDistance);
        }

        return treesArray;
    }

    public float GenerateRandomForest(float rowPosition)
    {
        bool isLeft = false;
        if (Random.Range(0, 2) == 0)
        {
            isLeft = true;
        }
        Vector3[][] positions = GenerateDoubleArray(rowPosition, isLeft);

        for (int i = 0; i < positions.Length; i++)
        {
            for (int k = 0; k < positions[i].Length; k++)
            {
                if (!positions[i][k].Equals(Vector3.zero))
                {
                    GameObject tree = RandomTree();

                    Transform spawned = Instantiate(tree.transform, positions[i][k], Quaternion.identity);
                    spawned.parent = obstaclesParent;
                }
            }
        }

        currentRowPosition += afterSpace;

        // update rowObstacleAIValues list with new values 
        float zRef = currentRowPosition - afterSpace - beforeSpace - rowPosition; // length of the forest
        int div = Mathf.CeilToInt(zRef / obstacleSpaceLength);


        float zPosInitial = rowPosition + beforeSpace + (obstacleSpaceLength / 2);

        for (int i = 0; i < div; i++) // go through each supposed row in the forest
        {
            float[] AIValues = new float[1 + obstacleSpacesInRow];
            AIValues[0] = zPosInitial + (obstacleSpaceLength / 2) + (i * obstacleSpaceLength);

            if (isLeft)
            {
                for (int k = 1; k < 1 + (obstacleSpacesInRow / 2); k++)
                {
                    AIValues[k] = -1;
                }
            }
            else
            {
                for (int k = 1 + (obstacleSpacesInRow / 2); k < 1 + obstacleSpacesInRow; k++)
                {
                    AIValues[k] = -1;
                }
            }

            rowObstacleAIValues.Add(AIValues);
        }


        return currentRowPosition;
    }


    void Initialize() // initialize variables 
    {
        obstacleSpacesInRow = obstacleSpacesInARow;
        obstacleSpaceLength = obstacleSpaceLengthValue;

        lastPositionPlaced = new float[obstacles.Length];
        width = maxWidth - minWidth;

        obstacleSpaceInRowWidth = width / obstacleSpacesInRow;
        rowPosition = startPosition;

        rowSpacesCenterPosition = new float[obstacleSpacesInRow];

        float centerPos = minWidth;
        for (int i = 0; i < rowSpacesCenterPosition.Length; i++) // initializing rowSpacesCenterPosition array
        {
            if (i == 0)
            {
                centerPos += obstacleSpaceInRowWidth / 2;
                rowSpacesCenterPosition[i] = centerPos;
            }
            else
            {
                centerPos += obstacleSpaceInRowWidth;
                rowSpacesCenterPosition[i] = centerPos;
            }
        }

        // initialize rowObstacleAIValue double array of floats
        rowObstacleAIValues = new List<float[]>();

        // initialize some variables for the random forest generation
        randomForestTotalLength = beforeSpace + afterSpace + (randomForestRows * randomForestMaxTreeLengthDistance);

    }


    private void Awake()
    {
        Initialize();
    }

    public void SpawnObstacles()
    {

        while ((rowPosition + maxDistanceBetweenRows) < startPosition + (meshZSize)) // checks if the position of this row is inside the spawned terrain (doesn't go overboard)
        {
            if (!isEndless && (rowPosition + maxDistanceBetweenRows) >= endPosition) // if we are not in endless, we stop spawning obstacles after endPosition
            {
                return;
            }
            if (Random.Range(0, chance) == 0)
            {
                if (rowPosition + randomForestTotalLength < startPosition + (meshZSize))
                {
                    rowPosition = GenerateRandomForest(rowPosition);
                }
                else
                {
                    SpawnRowOfObstacles();
                }
            }
            else
            {
                SpawnRowOfObstacles();
            }
        }
    }

    void SpawnRowOfObstacles()
    {
        rowPosition = NextRowPosition(); // increment rowPosition
        timesPlacedInRow = new int[obstacles.Length]; // reset times placed in row array
        bool[] spacesAvailable = RowSpaceAvailable(); // receives a boolean array with the row spaces we will use, ex: if we have a minimum of 2 obstacles per row, we need to receive at least 2 true values {false, false, true, true}, these values mean that first and second spaces have no obstacle and third and forth have one each

        float[] AIValues = new float[1 + obstacleSpacesInRow];
        AIValues[0] = rowPosition;

        for (int i = 0; i < spacesAvailable.Length; i++)
        {
            if (spacesAvailable[i])
            {
                float height = FindHeight(new Vector3(rowSpacesCenterPosition[i], 20, rowPosition)); // finds terrain height
                Vector3 position = new Vector3(rowSpacesCenterPosition[i], height, rowPosition);
                // spawning the obstacle, setting its parent and adding it to the list rowobstacleaivalue
                Transform spawned = Instantiate(RandomObstacle().transform, position, Quaternion.identity);
                spawned.parent = obstaclesParent;
                if(spawned.name.Equals("Booster(Clone)") || spawned.name.Equals("Ramp 1(Clone)") || spawned.name.Equals("Ramp 2(Clone)"))
                {
                    AIValues[i + 1] = 1;
                }
                else if(spawned.name.Equals("Fence 1(Clone)") || spawned.name.Equals("Fence 2(Clone)") || spawned.name.Equals("FenceShort(Clone)"))
                {
                    AIValues[i + 1] = -1;
                }
                else if(spawned.name.Equals("Tree 1(Clone)") || spawned.name.Equals("Tree 2(Clone)") || spawned.name.Equals("Tree 3(Clone)"))
                {
                    AIValues[i + 1] = -1;
                }
            }
        }
        rowObstacleAIValues.Add(AIValues);

    }

    float NextRowPosition() // calculates a random value, increments rowPosition and returns it
    {
        float distanceForNextRow = Random.Range(minDistanceBetweenRows, maxDistanceBetweenRows);
        rowPosition += distanceForNextRow;

        return rowPosition;
    }

    bool[] RowSpaceAvailable() // returns a boolean array that tells us which spaces of the row we will spawn obstacles in
    {
        bool[] spaces = new bool[obstacleSpacesInRow];
        int chosenSpaces = 0; // when we choose a space, we increment this
        for (int i = 0; i < spaces.Length; i++)
        {
            if (minObstaclesInRow > chosenSpaces && (spaces.Length + chosenSpaces) - minObstaclesInRow == i) // we have limited spaces left and a minimum of obstacles to be in this row and none chosen yet, we have to place objects on the spaces remaining
            {
                for (int k = i; k < spaces.Length; k++)
                {
                    spaces[k] = true;
                    chosenSpaces++;
                }

                return spaces;
            }

            if (Random.Range(0, obstacleSpacesInRow - minObstaclesInRow + 1) == 0) // random chance
            {
                spaces[i] = true;
                chosenSpaces++;
            }

            if (chosenSpaces == maxObstaclesInRow) // if we already have enough obstacles
            {
                break;
            }
        }

        return spaces;
    }

    GameObject RandomObstacle() // returns a random available obstacle for our row
    {
        bool[] obstacleAvailable = new bool[obstacles.Length]; // used to check which obstacles are available
        int cont = 0;
        for (int i = 0; i < obstacles.Length; i++) // checks which obstacles are available based on distance with last time they spawned
        {
            if (lastPositionPlaced[i] + minDistanceBetweenSameObstacles[i] < rowPosition)
            {
                obstacleAvailable[i] = true;
                cont++;
            }
        }

        for (int i = 0; i < obstacles.Length; i++) // checks which obstacles are available based on how many times they have spawned in the same row
        {
            if (timesPlacedInRow[i] >= maxRowAmmount[i])
            {
                if (obstacleAvailable[i] == true)
                {
                    obstacleAvailable[i] = false;
                    cont--;
                }
            }
        }

        int lastAvailable = 0;
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacleAvailable[i])
            {
                lastAvailable = i;
                if (Random.Range(0, cont) == 0) // we use chance for each obstacle that is available to be spawned
                {
                    lastPositionPlaced[i] = rowPosition;
                    timesPlacedInRow[i]++;
                    return obstacles[i];
                }
            }
        }
        lastPositionPlaced[lastAvailable] = rowPosition; // if chance didnt work, we use the last available object we had
        return obstacles[lastAvailable];
    }

    public float FindHeight(Vector3 pos) // finds height in terrain
    {
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(pos, Vector3.down, out hit, 1000, groundLayer))
        {
            return hit.point.y;
        }

        return 0;
    }
}
