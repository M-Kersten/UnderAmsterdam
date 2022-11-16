using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class ioScript : MonoBehaviour
{
    public int width = 16;//x
    public int height = 3;//y
    public int depth = 23;//z

    private IOTileData[] northGrid;
    private IOTileData[] southGrid;
    private IOTileData[] eastGrid;
    private IOTileData[] westGrid;
    public Transform northWall, southWall, eastWall, westWall;

    // Start is called before the first frame update
    void Start()
    {

        // Random seed is set
        Random.InitState((int)System.DateTime.Now.Ticks);

        northGrid = new IOTileData[height * depth];
        southGrid = new IOTileData[height * depth];
        eastGrid = new IOTileData[height * width];
        westGrid = new IOTileData[height * width];

        // Filling the GameObject arrays with all the IOTiles
        int i = 0;
        foreach (Transform tile in northWall)
            northGrid[i++] = tile.gameObject.GetComponent<IOTileData>();
        i = 0;
        foreach (Transform tile in southWall)
            southGrid[i++] = tile.gameObject.GetComponent<IOTileData>();
        i = 0;
        foreach (Transform tile in eastWall)
            eastGrid[i++] = tile.gameObject.GetComponent<IOTileData>();
        i = 0;
        foreach (Transform tile in westWall)
            westGrid[i++] = tile.gameObject.GetComponent<IOTileData>();

        // Adding the two first Input and Output
        foreach (var player in CompanyManager.Instance._companies)
        {
            if (player.Value != CompanyManager.Instance.emptyPlayer)
            {
                addPipe(player.Key, true);
                addPipe(player.Key, false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            foreach (var player in CompanyManager.Instance._companies)
            {
                if (player.Value != CompanyManager.Instance.emptyPlayer)
                    addPipe(player.Key, false);
            }
            addPipe("water", false);
        }
    }

    private void addPipe(string company, bool isOutput)
    {
        bool placedPipe = false;
        int wallSelec, lineSelec, columnSelec;

        while (!placedPipe)
        {
            //Randomly Choosing the wall among the 4 ones and the coordinates
            wallSelec = Random.Range(0, 4);
            lineSelec = (int)(Random.value * height);
            columnSelec = (int)(Random.value * (wallSelec < 2 ? depth : width));

            //For each wall is checked if the pipe isn't already placed with these coordinates then activate it
            // ADD MORE WALLS IF NEEDED
            if (wallSelec == 0)
                placedPipe = northGrid[height * lineSelec + columnSelec].OnEnableIO(company, isOutput, wallSelec);
            else if (wallSelec == 1)
                placedPipe = southGrid[height * lineSelec + columnSelec].OnEnableIO(company, isOutput, wallSelec);
            else if (wallSelec == 2)
                placedPipe = eastGrid[height * lineSelec + columnSelec].OnEnableIO(company, isOutput, wallSelec);
            else
                placedPipe = westGrid[height * lineSelec + columnSelec].OnEnableIO(company, isOutput, wallSelec);
        }
    }
}
