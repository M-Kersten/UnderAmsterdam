using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MainGrid : MonoBehaviour
{
    // Keeps a track of the former inputs to deactivate them
    public IOTileData[] companiesInput;
    private int NUMBER_OF_COMPANIES = 5;
    private List<string> _companies = new List<string> { "water", "gas", "data", "sewage", "power" };

    void Start()
    {
        companiesInput = new IOTileData[NUMBER_OF_COMPANIES];
    }

    public void replaceInput(IOTileData input)
    {
        companiesInput[_companies.IndexOf(input.company)].isActive = false;
        companiesInput[_companies.IndexOf(input.company)] = input;
    }

}
