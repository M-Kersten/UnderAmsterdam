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
    [SerializeField] private NetworkObject[] neighbors;
    [SerializeField] private GameObject connectorPart;
    [SerializeField] private GameObject connectorPartPreview;
    [SerializeField] private GameObject linePreview;
    private PipeColouring pColouring;

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
            neighbors[(int)Direction.Up] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Up] = null;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            neighbors[(int)Direction.Down] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Down] = null;

        if (Physics.Raycast(transform.position, Vector3.left, out hit))
            neighbors[(int)Direction.Left] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Left] = null;

        if (Physics.Raycast(transform.position, Vector3.right, out hit))
            neighbors[(int)Direction.Right] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Right] = null;

        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
            neighbors[(int)Direction.Front] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Front] = null;

        if (Physics.Raycast(transform.position, Vector3.back, out hit))
            neighbors[(int)Direction.Behind] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Behind] = null;
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
                if (neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
                {
                    neighborTile.activatedPipes[GetOppositeFace(i)] = false;

                    if ((neighborTile.company != "Empty") && (neighborTile.company == company || neighborTile.company == playerCompany))
                    {
                        activatedPipes[i] = enable;
                        neighborTile.activatedPipes[GetOppositeFace(i)] = enable;
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
        company = "Empty";

        // Deactivate all Half-pipes of the tile
        connectorPart.SetActive(false);
        connectorPartPreview.SetActive(false);

        for (int i = 0; i < neighbors.Length; i++)
        {
            pipeParts[i].SetActive(false);
            previewPipeParts[i].SetActive(false);

            if (neighbors[i] != null && neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
            {
                neighborTile.pipeParts[GetOppositeFace(i)].SetActive(false);
                neighborTile.previewPipeParts[GetOppositeFace(i)].SetActive(false);
                neighborTile.activatedPipes[GetOppositeFace(i)] = false;
                neighborTile.TryShowConnector();
            }
        }

    }
    public void OnHandEnter(string playerCompany)
    {
        if (isSpawned && !TileOccupied)
        {
            isHover = true;
            handHoverNumber++;

            UpdateNeighborData(true , playerCompany);
            OnRenderPipePreview(true);
        }
    }
    public void OnHandExit(string playerCompany)
    {
        if (isSpawned && !TileOccupied)
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
            if (previewPipeParts[i] != null)
            {
                if (activatedPipes[i])
                {
                    //Display/undisplay every pipe which is activated
                    pipeParts[i].SetActive(isActive);

                    if (neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
                    {
                        if (neighborTile.activatedPipes[GetOppositeFace(i)])
                        {
                            neighborTile.pipeParts[GetOppositeFace(i)].SetActive(isActive);
                            neighborTile.pColouring.UpdateRenderer(company);
                            neighborTile.TryShowConnector();
                        }
                    }
                }
            }
        }
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

                if (neighbors[i] != null && neighbors[i].TryGetComponent(out CubeInteraction neighborTile) && neighborTile.activatedPipes[GetOppositeFace(i)])
                    neighborTile.previewPipeParts[GetOppositeFace(i)].SetActive(isActive);
            }
        }
    }

    // This code gets ran ON OTHER PLAYERS when a pipe has been placed, changed is the new values of the placed pipe
    static void OnPipeChanged(Changed<CubeInteraction> changed) // static because of networked var isPiped
    {
        bool isPipedCurrent = changed.Behaviour.TileOccupied;
    
        changed.Behaviour.OnPipeRender(isPipedCurrent);
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

    public void CheckConnectionForWin()
    {
        // For each neighbor...
        for (int i = 0; i < neighbors.Length; i++)
        {
            // if it's a normal tile...
            if (neighbors[i] != null) {
                if (neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
                {
                    // from the same company and not checked yet...
                    if (company == neighborTile.company && !neighborTile.isChecked)
                    {
                        // Verify its neighbor and mark it as checked.
                        isChecked = true;
                        neighborTile.CheckConnectionForWin();
                    }
                }
                // if it's an Output tile...
                else if (neighbors[i].TryGetComponent(out IOTileScript IOPipe))
                {
                    // from the same company and active and if it isnt output (aka where it came from)
                    if (company == IOPipe.company && IOPipe.gameObject.activeSelf && !IOPipe.isOutput)
                    {
                        WinLoseManager.Instance.AddInputTracker(company);
                        return;
                    }
                }
            }
        }
    } 
}
