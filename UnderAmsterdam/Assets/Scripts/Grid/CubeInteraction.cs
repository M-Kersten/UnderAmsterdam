using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;

public class CubeInteraction : NetworkBehaviour
{
    private enum Direction {Right, Left, Behind, Front, Up, Down};

    [SerializeField] private Transform PipePreview, PipeHolder;
    [SerializeField] private GameObject connectorPart;
    [SerializeField] private GameObject connectorPartPreview;
    [SerializeField] private GameObject linePreview;
    [SerializeField] private GameObject particles, particlesWin;
    private PipeColouring pColouring;
    private NetworkObject[] neighbors;
    private CubeInteraction[] neighborsScript;

    // When TileOccupied changes value, run OnPipeChanged function
    [Networked(OnChanged = nameof(OnPipeChanged))]
    public bool TileOccupied { get; set; } // can be changed and send over the network only by the host

    // When company's values changes, run OnCompanyChange
    [SerializeField]
    [Networked(OnChanged = nameof(onCompanyChange))]
    public string company { get; set; }

    [SerializeField] private GameObject[] pipeParts;
    [SerializeField] private GameObject[] previewPipeParts;
    [SerializeField] private bool[] activatedPipes;

    private int amountFaces = 6;
    private bool isSpawned = false;

    public bool playerInside = false;
    public bool obstructed = false;
    public bool isHover = false;
    private uint handHoverNumber = 0; // avoid enter/exit problem whith two hands
    public bool isChecked = false;

    void Start() {
        pColouring = GetComponent<PipeColouring>();
    }
    public override void Spawned()
    {
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
        isSpawned = true;

        company = "Empty";
    }

    private void GetNeighbors()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.up , out hit))
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

    static void onCompanyChange(Changed<CubeInteraction> changed)
    {
        // When company changes give the new company (changed is the new values)
        changed.Behaviour.UpdateCompany(changed.Behaviour.company);
        changed.Behaviour.UpdateNeighborData(true);
    }

    private void UpdateNeighborData(bool enable, string playerCompany = "")
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

                    if ((neighborsScript[i].company != "Empty") && (neighborsScript[i].company == company || neighborsScript[i].company == playerCompany))
                    {
                        activatedPipes[i] = enable;
                        neighborsScript[i].activatedPipes[GetOppositeFace(i)] = enable;
                    }
                }
                // Or the IO tile
                else if (neighbors[i].TryGetComponent(out IOTileScript IOTile))
                {
                    if (IOTile.company != "Empty" && (IOTile.company == company || IOTile.company == playerCompany))
                        activatedPipes[i] = enable;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rock"))
            obstructed = true;
        if (other.CompareTag("Player"))
            playerInside = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Rock"))
            obstructed = false;
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    [Tooltip("Should be activated before EnableTile()")]
    public void UpdateCompany(string newCompany) {
        company = newCompany;
        pColouring.UpdateRenderer(company);
    }

    public void EnableTile()
    {
        //Gamemanager.Instance.pManager.RemovePoints(company);
        TileOccupied = true;

        OnRenderPipePreview(false);
        UpdateNeighborData(true);
        OnRenderPipePart(true);
        pColouring.UpdateRenderer(company);
    }
    public void DisableTile()
    {
        //Gamemanager.Instance.pManager.AddPoints(company);
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
        company = "Empty";

        StartCoroutine(DisplayDebris());
    }
    public void OnHandEnter(string playerCompany)
    {
        if (isSpawned && !playerInside && !obstructed && !TileOccupied)
        {
            isHover = true;
            handHoverNumber++;

            UpdateNeighborData(true , playerCompany);
            OnRenderPipePreview(true);
        }
    }
    public void OnHandExit(string playerCompany)
    {
        if (isSpawned && !playerInside && !obstructed && !TileOccupied)
        {
            //Stop the preview only when both hands are no more inside a tile
            if(true)
            {
                isHover = false;
                OnRenderPipePreview(false);
            }
            handHoverNumber--;
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
                    neighborsScript[i].pColouring.UpdateRenderer(company);
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

    // This code gets ran ON OTHER PLAYERS when a pipe has been placed, changed is the new values of the placed pipe
    static void OnPipeChanged(Changed<CubeInteraction> changed) // static because of networked var isPiped
    {
        changed.Behaviour.OnPipeRender(changed.Behaviour.TileOccupied);
    }
    
    // Run this code locally for players where pipe hasn't changed yet
    void OnPipeRender(bool isPipedCurrent)
    {
        if (isPipedCurrent) EnableTile();
        else DisableTile();
    }

    private int GetOppositeFace(int i)
    {
        return i + 1 - 2 * (i % 2);            
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
        pColouring.UpdateRenderer(company, connectorPart);
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
                pipeParts[i].transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(true);
            }
        }else
        {
            if (neighborsScript[isLinePipe] != null)
            {
                pipeParts[isLinePipe].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                pipeParts[isLinePipe].transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
                pipeParts[isLinePipe].transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
                pipeParts[isLinePipe].transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
            }
            if (neighborsScript[GetOppositeFace(isLinePipe)] != null)
            {
                pipeParts[GetOppositeFace(isLinePipe)].transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
                pipeParts[GetOppositeFace(isLinePipe)].transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
                pipeParts[GetOppositeFace(isLinePipe)].transform.GetChild(2).transform.GetChild(0).gameObject.SetActive(false);
                pipeParts[GetOppositeFace(isLinePipe)].transform.GetChild(3).transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public bool CheckConnectionForWin()
    {
        // For each neighbor...
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null) {
                // if it's a normal tile...
                if (neighborsScript[i] != null)
                {
                    // from the same company and not checked yet...
                    if (company == neighborsScript[i].company && !neighborsScript[i].isChecked)
                    {
                        // Verify this neighbor and mark it as checked.
                        isChecked = true;
                        if (neighborsScript[i].CheckConnectionForWin())
                        {
                            StartCoroutine(BurstFireWorks());
                            return true;
                        }
                        else return false;
                    }
                }
                // if it's an Output tile...
                else if (neighbors[i].TryGetComponent(out IOTileScript IOPipe))
                {
                    // from the same company and active and if it isnt output (aka where it came from)
                    if (company == IOPipe.company && IOPipe.gameObject.activeSelf && !IOPipe.isOutput && IOPipe.roundInputPipe == Gamemanager.Instance.currentRound)
                    {
                        // Add points to this company
                        // Gamemanager.Instance.pManager.AddPoints(company.Key);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator DisplayDebris()
    {
        GameObject instance = Instantiate(particles, transform);
        yield return new WaitForSeconds(1f);
        Destroy(instance);
        yield return null;
    }
    private IEnumerator BurstFireWorks()
    {
        GameObject instance = Instantiate(particlesWin, transform);
        yield return new WaitForSeconds(0.9f);
        Destroy(instance);
        yield return null;
    }

}

