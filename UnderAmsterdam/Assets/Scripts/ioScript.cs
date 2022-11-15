using UnityEngine;

public class ioScript : MonoBehaviour
{
    public int width = 16;//x
    public int height = 3;//y
    public int depth = 23;//z

    private IOTileScript[] eastGrid;
    private IOTileScript[] westGrid;
    public Transform eastWall;
    public Transform westWall;

    // Start is called before the first frame update
    void Start()
    {
        // Random seed is set
        Random.InitState((int)System.DateTime.Now.Ticks);

        eastGrid = new IOTileScript[height * width];
        westGrid = new IOTileScript[height * width];

        // Filling the GameObject arrays with all the IOTiles
        int i = 0;
        foreach (Transform tile in eastWall)
            eastGrid[i++] = tile.gameObject.GetComponent<IOTileScript>();
        i = 0;
        foreach (Transform tile in westWall)
            westGrid[i++] = tile.gameObject.GetComponent<IOTileScript>();

        Gamemanager.Instance.GameStart.AddListener(AddPlayerOutputs);
        Gamemanager.Instance.RoundStart.AddListener(AddPlayerInputs);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            AddPlayerInputs();
        }
    }

    private void AddPlayerInputs()
    {
        foreach (var player in CompanyManager.Instance._companies)
        {
            if (player.Value != CompanyManager.Instance.emptyPlayer)
            {
                PlaceInputPipe(player.Key);
            }
        }
    }

    private void PlaceInputPipe(string company)
    {
        bool placedInput = false;
        int wallSelect, lineSelect, columnSelect;

        while (!placedInput)
        {
            //Randomly Choosing the wall among the 4 ones and the coordinates
            wallSelect = Random.Range(0, 2);
            lineSelect = (int)(Random.value * height);
            columnSelect = (int)(Random.value * (wallSelect < 2 ? depth : width));

            //For each wall is checked if the pipe isn't already placed with these coordinates then activate it
            // ADD MORE WALLS IF NEEDED
            if (wallSelect == 0)
            {
                placedInput = eastGrid[height * lineSelect + columnSelect].TryEnableIOPipe(company, false);
            }
            else
            {
                placedInput = eastGrid[height * lineSelect + columnSelect].TryEnableIOPipe(company, false);
            }
        }
    }

    //this is just to place output pipes randomly at start, its doubble code and it sucks
    private void AddPlayerOutputs()
    {
        foreach (var player in CompanyManager.Instance._companies)
        {
            if (player.Value != CompanyManager.Instance.emptyPlayer)
            {
                bool placedInput = false;
                int wallSelect, lineSelect, columnSelect;

                while (!placedInput)
                {
                    //Randomly Choosing the wall among the 4 ones and the coordinates
                    wallSelect = Random.Range(0, 2);
                    lineSelect = (int)(Random.value * height);
                    columnSelect = (int)(Random.value * (wallSelect < 2 ? depth : width));

                    //For each wall is checked if the pipe isn't already placed with these coordinates then activate it
                    // ADD MORE WALLS IF NEEDED
                    if (wallSelect == 0)
                    {
                        placedInput = eastGrid[height * lineSelect + columnSelect].TryEnableIOPipe(player.Key, true);
                    }
                    else
                    {
                        placedInput = eastGrid[height * lineSelect + columnSelect].TryEnableIOPipe(player.Key, true);
                    }
                }
            }
        }
    }
}
