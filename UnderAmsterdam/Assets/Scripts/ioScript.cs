using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ioScript : MonoBehaviour
{
    [SerializeField] private List<IOTileScript> inputPipes;
    [SerializeField] private List<IOTileScript> outputPipes;

    private IOTileScript[] northGrid;
    private IOTileScript[] southGrid;
    private IOTileScript[] eastGrid;
    private IOTileScript[] westGrid;

    public Transform northWall, southWall, eastWall, westWall;

    private bool hasPlacedOutputs;

    // Start is called before the first frame update
    void Start()
    {
        // Random seed is set
        Random.InitState((int)System.DateTime.Now.Ticks);

        inputPipes = new List<IOTileScript>();
        outputPipes = new List<IOTileScript>();

        northGrid = new IOTileScript[northWall.childCount];
        southGrid = new IOTileScript[southWall.childCount];
        eastGrid = new IOTileScript[eastWall.childCount];
        westGrid = new IOTileScript[westWall.childCount];

        // Filling the GameObject arrays with all the IOTiles
        int i = 0;
        foreach (Transform tile in northWall)
            northGrid[i++] = tile.gameObject.GetComponent<IOTileScript>();
        i = 0;
        foreach (Transform tile in southWall)
            southGrid[i++] = tile.gameObject.GetComponent<IOTileScript>();
        i = 0;
        foreach (Transform tile in eastWall)
            eastGrid[i++] = tile.gameObject.GetComponent<IOTileScript>();
        i = 0;
        foreach (Transform tile in westWall)
            westGrid[i++] = tile.gameObject.GetComponent<IOTileScript>();

        Gamemanager.Instance.RoundStart.AddListener(FlushPreviousRoundData);
        Gamemanager.Instance.RoundStart.AddListener(AddPlayerInputs);
        Gamemanager.Instance.RoundStart.AddListener(AddPlayerOutputs);
        Gamemanager.Instance.RoundEnd.AddListener(StartCheckingPipes);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            //AddPlayerInputs();
            StartCheckingPipes();
            Debug.Log("starting checking process!!!!");
        }
    }

    private void FlushPreviousRoundData()
    {
        inputPipes.Clear();
    }

    private void StartCheckingPipes()
    {
        for(int i = 0; i < outputPipes.Count; i++)
        {
            outputPipes[i].StartPipeCheck();
        }
    }

    private void AddPlayerOutputs()
    {
        if (!hasPlacedOutputs)
        {
            foreach (var player in CompanyManager.Instance._companies)
            {
                if (player.Value != CompanyManager.Instance.emptyPlayer)
                {
                    hasPlacedOutputs = true;
                    outputPipes.Add(PlaceIOPipe(player.Key, true));
                }
            }
        }
    }

    private void AddPlayerInputs()
    {
        foreach (var player in CompanyManager.Instance._companies)
        {
            if (player.Value != CompanyManager.Instance.emptyPlayer)
            {
                inputPipes.Add(PlaceIOPipe(player.Key, false));
            }
        }
    }

    private IOTileScript PlaceIOPipe(string company, bool isOutput)
    {
        IOTileScript chosenTile;
        bool placedInput = false;
        int wallSelect, randomIndex;

        chosenTile = new IOTileScript();

        while (!placedInput)
        {
            //Randomly Choosing the wall among the 4 walls
            wallSelect = Random.Range(0, 4);
            
            //For each wall is checked if the pipe isn't already placed with these coordinates then activate it
            switch (wallSelect) {
                case 0:
                    randomIndex = Random.Range(0, northGrid.Length);
                    placedInput = northGrid[randomIndex].TryEnableIOPipe(company, isOutput);
                    Debug.Log(northGrid[randomIndex]);
                    if (placedInput)
                        chosenTile = northGrid[randomIndex];
                    break;
                case 1:
                    randomIndex = Random.Range(0, southGrid.Length);
                    placedInput = southGrid[randomIndex].TryEnableIOPipe(company, isOutput);
                    Debug.Log(southGrid[randomIndex]);
                    if (placedInput)
                        chosenTile = southGrid[randomIndex];
                    break;
                case 2:
                    randomIndex = Random.Range(0, westGrid.Length);
                    placedInput = westGrid[randomIndex].TryEnableIOPipe(company, isOutput);
                    Debug.Log(westGrid[randomIndex]);
                    if (placedInput)
                        chosenTile = westGrid[randomIndex]; 
                    break;
                case 3:
                    randomIndex = Random.Range(0, eastGrid.Length);
                    placedInput = eastGrid[randomIndex].TryEnableIOPipe(company, isOutput);
                    Debug.Log(eastGrid[randomIndex]);
                    if (placedInput)
                        chosenTile = eastGrid[randomIndex];
                    break;
            }
        }
        Debug.Log(chosenTile);
        return chosenTile;
    }
}
