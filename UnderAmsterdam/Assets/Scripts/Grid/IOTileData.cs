using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOTileData : MonoBehaviour
{
    public bool isActive = false;
    public string company;
    //TMP
    private List<string> _companies = new List<string> { "water", "gas", "data", "sewage", "power" };
    [SerializeField] private Material[] pipeMaterials;
    [SerializeField] private Renderer myRenderer;

    // To set the input pipe Game Object in the MainGrid script.
    public GameObject grid;
    private MainGrid gridScript;

    public GameObject[] companyPipes;

    private Vector3[] facedDirection = {Vector3.forward, Vector3.back, Vector3.left, Vector3.right};
    private CubeInteraction closeNeighbor;

    private int NUMBER_OF_COMPANIES = 1;

    void Start()
    {

        company = "Empty"; // No company
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
    public bool OnEnableIO(string setCompany, bool isOutput, int wallSelec)
    {
        if (company != "Empty") return false;

        company = setCompany;
        isActive = true;
        companyPipes[0].SetActive(true);
        myRenderer.material = pipeMaterials[_companies.IndexOf(company)];//TMP

        if (isOutput)
        {
            RaycastHit hit;

            // Gets the Tile towards which the pipe is facing
            Physics.Raycast(transform.position, facedDirection[wallSelec], out hit);
            closeNeighbor = hit.transform.gameObject.GetComponent<CubeInteraction>();
        }
        else { }
            //gridScript.replaceInput(gameObject.GetComponent<IOTileData>());

        return true;
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
