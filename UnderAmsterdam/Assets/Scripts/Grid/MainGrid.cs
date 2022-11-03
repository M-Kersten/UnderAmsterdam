using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;

public class MainGrid : MonoBehaviour
{
    //For the companies
    private CompaniesTMP[] companies;

    //For the pathFinding
    private bool[,,] isVisited;
    private NetworkObject currentTile;
    private NetworkObject neighborTile;
    private CubeInteraction cubeInter;
    private int i, tileCount = 0, returnValue;

    private int NUMBER_OF_COMPANIES = 5;
    private int NUMBER_OF_FACES_CUBE = 6;

    // Start is called before the first frame update
    void Start()
    {
        companies = new CompaniesTMP[NUMBER_OF_COMPANIES];
        //companies[0] = GetComponent<CompaniesTMP>(); // GET COMPANIES DATA HERE
    }

    private void OnTimeUp() //This function is called when the time is up for each round
    {
        //HERE RESET isVisited ARRAY :
        //isVisited[x, y, z] = false;

        for (i = 0; i < NUMBER_OF_COMPANIES; i++) // i is the selected company that is being checked
        {
            tileCount = 0;
            currentTile = companies[i].input; //Cheking starts from the input pipe of each company
            if (pathVerify(i) == 1)
                companies[i].nbPoints += 100;
        }

    }

    private int pathVerify(int companyNumber) //Recursive function
    {
        if (tileCount >= companies[companyNumber].nbTiles) //Failure to connect output to input
            return -1;
        if (currentTile == companies[companyNumber].output) //Success, output pipe has been reached
            return 1;

        Vector3 tilePos = currentTile.transform.position;
        isVisited[(int)(tilePos.x * 2f) + 2, (int)(tilePos.y * 2f) + 2, (int)(tilePos.z * 2f) + 2] = true; //TO VERIFY: ADJUST TO GRID POSITION

        cubeInter = currentTile.GetComponent<CubeInteraction>();

        for (i = 0; i < NUMBER_OF_FACES_CUBE; i++)
        {

            neighborTile = cubeInter.neighbors[i];

            Vector3 neighborPos = neighborTile.transform.position;
            if (neighborTile.GetComponent<CubeInteraction>().company == companyNumber && !isVisited[(int)(neighborPos.x*2f) + 2, (int)(neighborPos.y*2f) + 2, (int)(neighborPos.z*2f) + 2]) //Checks if it is the right company and if the pipe hasn't been visited yet
            {
                tileCount++;
                currentTile = neighborTile; //Transfers to one of its neighbor
            }

            returnValue = pathVerify(companyNumber);
            if (returnValue != 0) //Verify this neighbor as well until it return a stop value (-1 for Failure, 1 for Success)
                return returnValue;

        }

        return 0; //0 value means it is not a stop condition

    }

}
