using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOTileData : MonoBehaviour
{
    public bool isInput;
    public bool isActive;
    public int company;

    // To set the input pipe Game Object in the MainGrid script.
    public GameObject grid;
    private MainGrid gridScript;

    public GameObject[] companyPipes;

    private int NUMBER_OF_COMPANIES = 1;

    void Start()
    {
        company = -1; // No company
        companyPipes = new GameObject[NUMBER_OF_COMPANIES];

        // Deactivate all of the company pipes in the Tile
        int i = 0;
        foreach (Transform pipe in transform)
        {
            pipe.gameObject.SetActive(false);
            companyPipes[i++] = pipe.gameObject;
        }

        gridScript = grid.GetComponent<MainGrid>();
    }

    // Checks if the pipe isn't already placed with these coordinates then activate it.
    public bool OnEnableIO(int setCompany, bool input)
    {

        if (company != -1) return false;

        company = setCompany;
        isInput = input;
        companyPipes[company].SetActive(true);

        if (isInput) gridScript.companiesInput[company] = gameObject;

        return true;
    }
}
