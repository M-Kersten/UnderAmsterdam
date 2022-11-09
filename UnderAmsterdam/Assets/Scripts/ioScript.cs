using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ioScript : MonoBehaviour
{

    public int width = 16;//x
    public int height = 3;//y
    public int depth = 23;//z

    private GameObject[] eastGrid;
    private GameObject[] westGrid;
    public Transform eastWall;
    public Transform westWall;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);

        eastGrid = new GameObject[height * width];
        westGrid = new GameObject[height * width];

        int i = 0;
        foreach (Transform tile in eastWall)
        {
            eastGrid[i++] = tile.gameObject;
            tile.gameObject.SetActive(false);
        }
        i = 0;
        foreach (Transform tile in westWall)
        {
            westGrid[i++] = tile.gameObject;
            tile.gameObject.SetActive(false);
        }

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
        GameObject addedPipe = null;
        IOTileData pipeData;

        bool placedPipe = false;
        int wallSelec, lineSelec, columnSelec;

        while (!placedPipe)
        {
            //Randomly Choosing the wall among the 4 ones and the coordinates
            wallSelec = Random.Range(2, 4);
            lineSelec = Random.Range(0, height);
            columnSelec = Random.Range(0, wallSelec < 2 ? depth : width);

            //For each wall is checked if the pipe isn't already placed with these coordinates then activate it
            switch (wallSelec)
            {
                //DECOMMENT THIS PART IF THERE IS NORTH AND SOUTH WALLS
                /*
                case 0:
                    if (!northGrid[height * lineSelec + columnSelec].activeSelf)
                    {
                        placedPipe = true;
                        addedPipe = northGrid[height * lineSelec + columnSelec];
                        northGrid[height * lineSelec + columnSelec].SetActive(true);
                    }

                    break;
                case 1:
                    if (!southGrid[height * lineSelec + columnSelec].activeSelf)
                    {
                        placedPipe = true;
                        addedPipe = southGrid[height * lineSelec + columnSelec];
                        southGrid[height * lineSelec + columnSelec].SetActive(true);
                    }

                    break;*/
                case 2:
                    if (!eastGrid[height * lineSelec + columnSelec].activeSelf)
                    {
                        placedPipe = true;
                        addedPipe = eastGrid[height * lineSelec + columnSelec];
                        eastGrid[height * lineSelec + columnSelec].SetActive(true);
                    }
                    break;
                case 3:
                    if (!westGrid[height * lineSelec + columnSelec].activeSelf)
                    {
                        placedPipe = true;
                        addedPipe = westGrid[height * lineSelec + columnSelec];
                        westGrid[height * lineSelec + columnSelec].SetActive(true);
                    }
                    break;
            }
        }
        
        //CORRECT THIS PART WITH THE COMPANY CODE
        pipeData = addedPipe.GetComponent<IOTileData>();
        pipeData.isInput = isInput;
        pipeData.company = company;

    }
}
