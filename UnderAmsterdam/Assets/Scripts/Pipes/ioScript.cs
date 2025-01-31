using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion.XR.Host;

public class ioScript : MonoBehaviour
{
    public static ioScript Instance;

    [SerializeField] public List<CubeInteraction> tobeUncheckedPipes;

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
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Random seed is set
        Random.InitState((int)System.DateTime.Now.Ticks);

        inputPipes = new List<IOTileScript>();
        outputPipes = new List<IOTileScript>();
        tobeUncheckedPipes = new List<CubeInteraction>();

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

        if (Gamemanager.Instance.ConnectionManager.runner.IsServer)
            Gamemanager.Instance.GameStart.AddListener(AddPlayerOutputs);

        Gamemanager.Instance.RoundStart.AddListener(AddPlayerInputs);
        Gamemanager.Instance.RoundStart.AddListener(UncheckAllCheckedPipes);
        Gamemanager.Instance.RoundEnd.AddListener(StartCheckingPipes);
    }
    private void UncheckAllCheckedPipes()
    {
        for (int i = 0; i < tobeUncheckedPipes.Count; i++)
            tobeUncheckedPipes[i].isChecked = false;

        tobeUncheckedPipes.Clear();
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
        //Replace this to check if player OUTPUT already exist
        if (!hasPlacedOutputs)
        {
            foreach (var player in CompanyManager.Instance.Companies)
            {
                hasPlacedOutputs = true;
                outputPipes.Add(PlaceIOPipe(player.Key, true));
            }
        }
    }

    private void AddPlayerInputs()
    {
        foreach (var player in CompanyManager.Instance.Companies)
        {
            if (player.Value != CompanyManager.Instance.emptyPlayer)
            {
                inputPipes.Add(PlaceIOPipe(player.Key, false));
            }
        }
    }

    private IOTileScript PlaceIOPipe(int company, bool isOutput)
    {
        IOTileScript chosenTile;
        bool placedInput = false;
        int wallSelect, randomIndex;

        chosenTile = new IOTileScript();

        int attempts = 0;
        while (!placedInput && attempts < 1000)
        {
            attempts++;
            
            if (attempts == 10000) 
                Debug.Log("Max attempts reached without placing input!");
            
            if (isOutput)
            {
                randomIndex = Random.Range(0, westGrid.Length);
                placedInput = westGrid[randomIndex].TryEnableIOPipe(company, isOutput, false);
                if (placedInput)
                    chosenTile = westGrid[randomIndex];
            }
            else
            {
                wallSelect = Random.Range(0, 3);

                //For each wall is checked if the pipe isn't already placed with these coordinates then activate it
                switch (wallSelect)
                {
                    case 0:
                        randomIndex = Random.Range(0, northGrid.Length);
                        placedInput = northGrid[randomIndex].TryEnableIOPipe(company, isOutput, false);
                        if (placedInput)
                            chosenTile = northGrid[randomIndex];
                        break;
                    case 1:
                        randomIndex = Random.Range(0, southGrid.Length);
                        placedInput = southGrid[randomIndex].TryEnableIOPipe(company, isOutput, false);
                        if (placedInput)
                            chosenTile = southGrid[randomIndex];
                        break;
                    case 2:
                        randomIndex = Random.Range(0, eastGrid.Length);
                        placedInput = eastGrid[randomIndex].TryEnableIOPipe(company, isOutput, false);
                        if (placedInput)
                            chosenTile = eastGrid[randomIndex];
                        break;
                }
            }
        }
        return chosenTile;
    }
}
