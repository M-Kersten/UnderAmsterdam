using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;

public class MainGrid : MonoBehaviour
{
    // Keeps a track of the former inputs to deactivate them
    public IOTileData[] companiesInput;
    private int NUMBER_OF_COMPANIES = 1;

    void Start()
    {
        companiesInput = new IOTileData[NUMBER_OF_COMPANIES];
    }

    public void replaceInput(IOTileData input)
    {
        companiesInput[input.company].deactivateInput();
        companiesInput[input.company] = input;
    }

}
