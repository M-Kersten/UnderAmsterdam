using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOTileData : MonoBehaviour
{
    public bool isActive;
    public int company;

    // To set the input pipe Game Object in the MainGrid script.
    public GameObject grid;
    private MainGrid gridScript;

    public GameObject[] companyPipes;

    private CubeInteraction closeNeighbor;

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
    public bool OnEnableIO(int setCompany, bool isOutput, bool isWest)
    {
        if (company != -1) return false;

        company = setCompany;
        companyPipes[company].SetActive(true);

        if (isOutput)
        {
            RaycastHit hit;

            // Gets the Tile towards which the pipe is facing
            Physics.Raycast(transform.position, isWest ? Vector3.right : Vector3.left, out hit);
            closeNeighbor = hit.transform.gameObject.GetComponent<CubeInteraction>();
        }
        else
            gridScript.replaceInput(this);

        return true;
    }

    public void deactivateInput()
    {
        isActive = false;
    }

    public void startCheckWin()
    {
        // Starts the win condition check.
        if (company == closeNeighbor.company) closeNeighbor.checkWin();
    }

    public void winGameEvent()
    {
        // WIN CONDITION HERE OR ELSEWHERE IDK
        return;
    }
}
