using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;

public class CubeInteraction : NetworkBehaviour
{
    private enum Direction { Right, Left, Behind, Front, Up, Down };

    [SerializeField] private Transform PipePreview, PipeHolder;
    [SerializeField] private GameObject connectorPart;
    [SerializeField] private GameObject connectorPartPreview, redDot;
    [SerializeField] private GameObject linePreview;
    [SerializeField] private GameObject particles, particlesWin;
    [SerializeField] private bool practiceArea;

    private PipeColouring pColouring;
    private NetworkObject[] neighbors;
    private CubeInteraction[] neighborsScript;

    public bool TileOccupied;

    [Networked]
    public int Company { get; set; }

    [SerializeField] private GameObject[] pipeParts;
    [SerializeField] private GameObject[] previewPipeParts;
    [SerializeField] private bool[] activatedPipes;

    private int amountFaces = 6;
    private bool isSpawned = false;

    public bool playerInside = false;
    public bool obstructed = false;
    public bool isChecked = false;
    public bool isPlayTile;
    
    private ChangeDetector _changes;

    void Start()
    {
        pColouring = GetComponent<PipeColouring>();
        Gamemanager.Instance.RoundEnd.AddListener(delegate { OnRenderPipePreview(false); });
    }
    
    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(Company):
                    OnCompanyChange();
                    break;
            }
        }
    }
    
    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        ResetActivatedPipes();
        OnRenderPipePreview(false);
        OnRenderPipePart(false);

        neighbors = new NetworkObject[amountFaces]; //Cubes have 6 faces, thus we will always need 6 neigbors
        neighborsScript = new CubeInteraction[amountFaces];
        GetNeighbors();

        pipeParts = new GameObject[neighbors.Length];
        previewPipeParts = new GameObject[neighbors.Length];

        int i = 0;
        foreach (Transform pipe in PipeHolder)
            pipeParts[i++] = pipe.gameObject;
        i = 0;
        foreach (Transform pipePreview in PipePreview)
            previewPipeParts[i++] = pipePreview.gameObject;

        activatedPipes = new bool[neighbors.Length]; //Array of booleans storing which orientation is enabled [N, S, E, W, T, B]

        Company = -1;
        isSpawned = true;
    }

    private void GetNeighbors()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.up, out hit))
        {
            neighbors[(int)Direction.Up] = hit.transform.gameObject.GetComponent<NetworkObject>();
            neighborsScript[(int)Direction.Up] = hit.transform.gameObject.GetComponent<CubeInteraction>();
        }

        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            neighbors[(int)Direction.Down] = hit.transform.gameObject.GetComponent<NetworkObject>();
            neighborsScript[(int)Direction.Down] = hit.transform.gameObject.GetComponent<CubeInteraction>();
        }

        if (Physics.Raycast(transform.position, Vector3.left, out hit))
        {
            neighbors[(int)Direction.Left] = hit.transform.gameObject.GetComponent<NetworkObject>();
            neighborsScript[(int)Direction.Left] = hit.transform.gameObject.GetComponent<CubeInteraction>();
        }

        if (Physics.Raycast(transform.position, Vector3.right, out hit))
        {
            neighbors[(int)Direction.Right] = hit.transform.gameObject.GetComponent<NetworkObject>();
            neighborsScript[(int)Direction.Right] = hit.transform.gameObject.GetComponent<CubeInteraction>();
        }

        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
        {
            neighbors[(int)Direction.Front] = hit.transform.gameObject.GetComponent<NetworkObject>();
            neighborsScript[(int)Direction.Front] = hit.transform.gameObject.GetComponent<CubeInteraction>();
        }

        if (Physics.Raycast(transform.position, Vector3.back, out hit))
        {
            neighbors[(int)Direction.Behind] = hit.transform.gameObject.GetComponent<NetworkObject>();
            neighborsScript[(int)Direction.Behind] = hit.transform.gameObject.GetComponent<CubeInteraction>();
        }
    }

    private void OnCompanyChange()
    {
        // When company changes give the new company (changed is the new values)
        UpdateCompany(Company);
        UpdateNeighborData(true);
    }

    private void UpdateNeighborData(bool enable, int playerCompany = -2)
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            activatedPipes[i] = false;

            if (neighbors[i] != null)
            {
                // Check all of its neighbors and activate the corresponding pipes if it's from the same company
                if (neighborsScript[i] != null)
                {
                    neighborsScript[i].activatedPipes[GetOppositeFace(i)] = false;

                    if (neighborsScript[i].Company != -1 && (neighborsScript[i].Company == Company || neighborsScript[i].Company == playerCompany))
                    {
                        activatedPipes[i] = enable;
                        neighborsScript[i].activatedPipes[GetOppositeFace(i)] = enable;
                    }
                }
                // Or the IO tile
                else if (neighbors[i].TryGetComponent(out IOTileScript IOTile))
                {
                    if (IOTile.Company != -1 && (IOTile.Company == Company || IOTile.Company == playerCompany))
                        activatedPipes[i] = enable;
                }
            }
        }
    }

    //Checks if each neighbor is compatible with player's company
    public bool VerifyRules(int pCompany)
    {
        if (!isPlayTile)
        {
            if (pCompany == -1)
                return true;
        }

        for (int i = 0; i < neighborsScript.Length; i++)
        {
            if (neighbors[i] == null) continue;

            if (neighborsScript[i] == null)
            {
                //If there is no normal tiles, check for IOTiles
                if (neighbors[i].TryGetComponent(out IOTileScript IOTile) && IOTile.Company != -1 && IOTile.Company != pCompany) 
                    return false;
                continue;
            }
            else if (neighborsScript[i].Company == -1) 
                continue;

            if (//Incompatible company pairs, add more to implement more rules
                AreMatchingPairs(pCompany, neighborsScript[i].Company, (int)CompanyType.Water, (int)CompanyType.Power) ||
                AreMatchingPairs(pCompany, neighborsScript[i].Company, (int)CompanyType.Water, (int)CompanyType.Data) ||
                AreMatchingPairs(pCompany, neighborsScript[i].Company, (int)CompanyType.Water, (int)CompanyType.Sewage) ||
                AreMatchingPairs(pCompany, neighborsScript[i].Company, (int)CompanyType.Power, (int)CompanyType.Sewage) ||
                AreMatchingPairs(pCompany, neighborsScript[i].Company, (int)CompanyType.Data, (int)CompanyType.Sewage) ||
                AreMatchingPairs(pCompany, neighborsScript[i].Company, (int)CompanyType.Data, (int)CompanyType.Gas) ||
                AreMatchingPairs(pCompany, neighborsScript[i].Company, (int)CompanyType.Power, (int)CompanyType.Gas)
            ) return false;
        }

        return true;
    }

    private bool AreMatchingPairs(int playerCompany, int neighborCompany, int companyA, int companyB)
    {
        return (playerCompany == companyA && neighborCompany == companyB || playerCompany == companyB && neighborCompany == companyA);
    }

    private void OnTriggerEnter(Collider other)
    {
        ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit);

        if (other.CompareTag("Rock") || other.CompareTag("Root"))
        {
            obstructed = true;
            OnRenderPipePreview(false);
        }
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            OnRenderPipePreview(false);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);

        if (other.CompareTag("Rock") || other.CompareTag("Root"))
            obstructed = false;
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    [Tooltip("Should be activated before EnableTile()")]
    public void UpdateCompany(int newCompany)
    {
        Company = newCompany;
        pColouring.UpdateRenderer(Company);
    }

    public void EnableTile()
    {
        TileOccupied = true;

        OnRenderPipePreview(false);
        UpdateNeighborData(true);
        OnRenderPipePart(true);
        pColouring.UpdateRenderer(Company);
    }
    
    public void DisableTile()
    {
        // Clear company and occupation state
        TileOccupied = false;

        // Deactivate all Half-pipes of the tile
        connectorPart.SetActive(false);
        connectorPartPreview.SetActive(false);

        for (int i = 0; i < neighbors.Length; i++)
        {
            pipeParts[i].SetActive(false);
            previewPipeParts[i].SetActive(false);

            if (neighbors[i] != null && neighborsScript[i] != null)
            {
                neighborsScript[i].pipeParts[GetOppositeFace(i)].SetActive(false);
                neighborsScript[i].previewPipeParts[GetOppositeFace(i)].SetActive(false);
                neighborsScript[i].activatedPipes[GetOppositeFace(i)] = false;
                neighborsScript[i].TryShowConnector();
                neighborsScript[i].TryExtendPipe();
            }
        }
        Company = -1;

        Instantiate(particles, transform);
    }
    
    public void OnHandEnter(int playerCompany)
    {
        if (isSpawned && !playerInside && !obstructed && !TileOccupied)
        {
            if (VerifyRules(playerCompany))
            {
                UpdateNeighborData(true, playerCompany);
                OnRenderPipePreview(true);
            }
            else redDot.SetActive(true);

        }
    }
    
    public void OnHandExit(int playerCompany)
    {
        if (isSpawned && !playerInside && !obstructed && !TileOccupied)
        {
            //Stop the preview only when both hands are no more inside a tile
            if (VerifyRules(playerCompany)) OnRenderPipePreview(false);
            else redDot.SetActive(false);
        }
    }

    private void OnRenderPipePart(bool isActive)
    {

        if (!isSpawned)
            return;

        //connectorPart.SetActive(isActive);
        //pColouring.UpdateRenderer(company, connectorPart);

        if (!isActive) connectorPart.gameObject.SetActive(false);
        else TryShowConnector();

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (previewPipeParts[i] != null && activatedPipes[i])
            {
                //Display/undisplay every pipe which is activated
                pipeParts[i].SetActive(isActive);

                if (neighborsScript[i] != null && neighborsScript[i].activatedPipes[GetOppositeFace(i)])
                {
                    neighborsScript[i].pipeParts[GetOppositeFace(i)].SetActive(isActive);
                    neighborsScript[i].pColouring.UpdateRenderer(Company);
                    neighborsScript[i].TryShowConnector();
                    neighborsScript[i].TryExtendPipe();
                }

            }
        }
        TryExtendPipe();
    }

    private void OnRenderPipePreview(bool isActive)
    {
        if (!isSpawned)
            return;

        connectorPartPreview.SetActive(isActive);
        linePreview.SetActive(isActive);

        for (int i = 0; i < neighbors.Length; i++)
        {
            if (previewPipeParts[i] != null && (activatedPipes[i] || !isActive))
            {
                //Display/undisplay every pipe which is activated
                previewPipeParts[i].SetActive(isActive);

                if (neighbors[i] != null && neighborsScript[i] != null && neighborsScript[i].activatedPipes[GetOppositeFace(i)])
                    neighborsScript[i].previewPipeParts[GetOppositeFace(i)].SetActive(isActive);
            }
        }
    }

    private int GetOppositeFace(int index)
    {
        return index + 1 - 2 * (index % 2);
    }

    private void ResetActivatedPipes()
    {
        for (int i = 0; i < activatedPipes.Length; i++)
            activatedPipes[i] = false;
    }

    public void TryShowConnector()
    {
        // Checks if it is at least a line pipe
        for (int i = 0; i < amountFaces; i += 2)
        {
            if (!TileOccupied || pipeParts[i].activeSelf && pipeParts[GetOppositeFace(i)].activeSelf)
            {
                // Connector is not visible
                connectorPart.SetActive(false);
                return;
            }
        }
        // Connector is visible, it must be activated
        connectorPart.SetActive(true);
        pColouring.UpdateRenderer(Company, connectorPart);
    }

    private int IsLinePipe()
    {
        int lineFound = -1;
        for (int i = 0; i < amountFaces; i += 2)
        {
            if (pipeParts[i].activeSelf)
            {
                if (lineFound == -1 && pipeParts[GetOppositeFace(i)].activeSelf) lineFound = i;
                else return -1;
            }
            else if (pipeParts[GetOppositeFace(i)].activeSelf) return -1;
        }
        return lineFound;
    }

    public void TryExtendPipe()
    {
        int isLinePipe = IsLinePipe();
        bool enable = isLinePipe == -1 ? true : false;
        if (enable)
        {
            for (int i = 0; i < amountFaces; i++)
            {
                pipeParts[i].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                pipeParts[i].transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
                pipeParts[i].transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(true);
                pipeParts[i].transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
                pipeParts[i].transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(true);
                pipeParts[i].transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(true);
            }
        }
        else
        {
            if (neighborsScript[isLinePipe] != null)
            {
                pipeParts[isLinePipe].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                pipeParts[isLinePipe].transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
                pipeParts[isLinePipe].transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
                pipeParts[isLinePipe].transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
                pipeParts[isLinePipe].transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
                pipeParts[isLinePipe].transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
            }
            if (neighborsScript[GetOppositeFace(isLinePipe)] != null)
            {
                pipeParts[GetOppositeFace(isLinePipe)].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                pipeParts[GetOppositeFace(isLinePipe)].transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
                pipeParts[GetOppositeFace(isLinePipe)].transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
                pipeParts[GetOppositeFace(isLinePipe)].transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
                pipeParts[GetOppositeFace(isLinePipe)].transform.GetChild(3).transform.GetChild(0).gameObject.SetActive(false);
                pipeParts[GetOppositeFace(isLinePipe)].transform.GetChild(3).transform.GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    public void CheckConnectionForWin()
    {
        // For each neighbor...
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null)
            {
                // if it's a normal tile...
                if (neighborsScript[i] != null)
                {
                    // from the same company and not checked yet...
                    if (Company == neighborsScript[i].Company && !neighborsScript[i].isChecked)
                    {
                        // Verify this neighbor and mark it as checked.
                        isChecked = true;
                        ioScript.Instance.tobeUncheckedPipes.Add(this);
                        neighborsScript[i].CheckConnectionForWin();
                    }
                }
                // if it's an Output tile...
                else if (neighbors[i].TryGetComponent(out IOTileScript IOPipe))
                {
                    // from the same company and active and if it isnt output (aka where it came from)
                    if (Company == IOPipe.Company && IOPipe.gameObject.activeSelf && !IOPipe.isOutput && IOPipe.roundInputPipe == Gamemanager.Instance.currentRound)
                    {
                        // Add points to this company
                        Gamemanager.Instance.pManager.CalculateRoundPoints(Company);
                        TeamworkManager.Instance.CompanyDone(Company);
                        Instantiate(particlesWin, transform);

                        //Flickering lights
                        /*if (company == "power")
                            RandManager.Instance.addPowerPts();*/

                        return;
                    }
                }
            }
        }
    }
}