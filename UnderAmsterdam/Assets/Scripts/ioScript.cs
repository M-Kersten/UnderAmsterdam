using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ioScript : MonoBehaviour
{
    public GameObject player;
    private int company;

    public int width = 16;//x
    public int height = 3;//y
    public int depth = 23;//z

    private IOTileData[] eastGrid;
    private IOTileData[] westGrid;
    public Transform eastWall;
    public Transform westWall;

    // Start is called before the first frame update
    void Start()
    {
        //company = player.GetComponent<PlayerData>().company; // GET THE PLAYER COMPANY HERE

        // Random seed is set
        Random.InitState((int)System.DateTime.Now.Ticks);

        eastGrid = new IOTileData[height * width];
        westGrid = new IOTileData[height * width];

        // Filling the GameObject arrays with all the IOTiles
        int i = 0;
        foreach (Transform tile in eastWall)
            eastGrid[i++] = tile.gameObject.GetComponent<IOTileData>();
        i = 0;
        foreach (Transform tile in westWall)
            westGrid[i++] = tile.gameObject.GetComponent<IOTileData>();

        // Adding the two first Input and Output
        addPipe(0, true);
        addPipe(0, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            addPipe(0, false);
        }
    }

    private void addPipe(int company, bool isInput)
    {
        bool placedPipe = false;
        int wallSelec, lineSelec, columnSelec;

        while (!placedPipe)
        {
            //Randomly Choosing the wall among the 4 ones and the coordinates
            wallSelec = Random.Range(0, 2);
            lineSelec = (int)(Random.value * height);
            columnSelec = (int)(Random.value * (wallSelec < 2 ? depth : width));

            //For each wall is checked if the pipe isn't already placed with these coordinates then activate it
            // ADD MORE WALLS IF NEEDED
            if (wallSelec == 0)
                placedPipe = eastGrid[height * lineSelec + columnSelec].OnEnableIO(company, isInput);
            else
                placedPipe = westGrid[height * lineSelec + columnSelec].OnEnableIO(company, isInput);
        }
    }
}
