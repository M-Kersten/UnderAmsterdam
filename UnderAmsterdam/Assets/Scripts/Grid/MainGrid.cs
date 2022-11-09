using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;

public class MainGrid : MonoBehaviour
{
    //For the companies
    public GameObject[] companiesInput;
    public int[] companiesTilesNb; // NEEDS TO BE FILLED

    //For the pathFinding
    private bool[,,] isVisited;
    private int tileCount, returnValue;

    public int width = 16, height = 3, depth = 23;

    private int NUMBER_OF_COMPANIES = 5;
    private int NUMBER_OF_FACES_CUBE = 6;

    // Start is called before the first frame update
    void Start()
    {
        isVisited = new bool[width, height, depth];
        companiesInput = new GameObject[NUMBER_OF_COMPANIES];
        companiesTilesNb = new int[NUMBER_OF_COMPANIES];
    }

    private void OnTimeUp() //This function is called when the time is up for each round
    {
        int i;

        // Resets the visited tiles array
        for (i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < depth; k++)
                    isVisited[i, j, k] = false;
            }
        }

        for (i = 0; i < NUMBER_OF_COMPANIES; i++) // i is the selected company that is being checked
        {
            tileCount = 0;
            //Cheking starts from the input pipe of each company
            if (pathVerify(i, companiesInput[i]) == 1)
                winCondition();
        }

    }

    private int pathVerify(int companyNumber, GameObject currentTile) //Recursive function
    {
        IOTileData ioTile = currentTile.GetComponent<IOTileData>();
        if (ioTile != null && !ioTile.isInput && ioTile.company == companyNumber) //Verify if it is an output pipe from the same company
        {
            if (ioTile.isActive)
                return 1; //Success, output pipe has been reached
            return 0; //0 return value basically means to continue researches
        }

        if (tileCount >= companiesTilesNb[companyNumber]) //Failure to connect output to input (All the tiles have been verified)
            return -1;

        //Mark tile as verified 
        Vector3 tilePos = currentTile.transform.position;
        isVisited[(int)(tilePos.x * 2f) + 2, (int)(tilePos.y * 2f) + 2, (int)(tilePos.z * 2f) + 2] = true;

        CubeInteraction cubeInter = currentTile.GetComponent<CubeInteraction>();
        GameObject neighborTile;

        for (int i = 0; i < NUMBER_OF_FACES_CUBE; i++)
        {

            neighborTile = cubeInter.neighbors[i].gameObject;

            Vector3 neighborPos = neighborTile.transform.position;

            //Checks if it is the right company and if the pipe hasn't been visited yet
            if (neighborTile.GetComponent<CubeInteraction>().company != companyNumber || isVisited[(int)(neighborPos.x * 2f) + 2, (int)(neighborPos.y * 2f) + 2, (int)(neighborPos.z * 2f) + 2])
                continue;
            
            tileCount++;

            returnValue = pathVerify(companyNumber, neighborTile);
            if (returnValue != 0) //Verify this neighbor as well until it return a stop value (-1 for Failure, 1 for Success)
                return returnValue;

        }

        return 0;

    }

    private void winCondition()
    {
        // WIN CONDITION CODE HERE
    }

}
