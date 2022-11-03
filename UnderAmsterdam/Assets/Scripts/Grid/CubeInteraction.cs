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

    [SerializeField] [Networked(OnChanged = nameof(onCompanyChange))] private string company {get; set;}

    [Networked(OnChanged = nameof(OnPipeChanged))]
    public bool TileOccupied { get; set; } // can be changed and send over the network only by the host

    [SerializeField] private GameObject[] pipeParts;
    [SerializeField] private GameObject[] previewPipeParts;
    [SerializeField] private bool[] activatedPipes;

    private int amountFaces = 6;

    public bool isHover = false;

    public override void Spawned()
    {
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

    private void OnTriggerEnter(Collider other)
    {
        if (!TileOccupied)
        {
            UpdateNeighborData(true);
            OnRenderPipePreview(true);
            isHover = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!TileOccupied)
        {
            UpdateNeighborData(false);
            OnRenderPipePreview(false);
            isHover = false;
        }
    }
    static void onCompanyChange(Changed<CubeInteraction> changed)
    {
        changed.Behaviour.UpdateCompany(changed.Behaviour.company);
        changed.Behaviour.UpdateNeighborData(true);
    }

    private void UpdateNeighborData(bool enable)
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null) {

                CubeInteraction neighborTile = neighbors[i].GetComponent<CubeInteraction>();

                if (neighborTile.company != "Empty" && (neighborTile.company == company || isHover == enable))
                {
                    activatedPipes[i] = enable;
                    neighborTile.activatedPipes[GetOppositeFace(i)] = enable;
                }
            }
        }
    }
    [Tooltip("Should be activated before EnableTile()")]
    public void UpdateCompany(string newCompany) {
        company = newCompany;
    }
    public void EnableTile()
    {
        if (TileOccupied)
            return;

        isHover = false;
        TileOccupied = true;
        UpdateNeighborData(true);
        OnRenderPipePart(true);
        OnRenderPipePreview(false);
    }

    private void OnRenderPipePart(bool isActive)
    {
        connectorPart.SetActive(isActive);
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (previewPipeParts[i] != null)
            {
                if (activatedPipes[i])
                {
                    //Display/undisplay every pipe which is activated
                    pipeParts[i].SetActive(isActive);

                    CubeInteraction neighborTile = neighbors[i].GetComponent<CubeInteraction>();

                    if (neighborTile.activatedPipes[GetOppositeFace(i)])
                        neighborTile.pipeParts[GetOppositeFace(i)].SetActive(isActive);
                }
            }
        }
    }

    private void OnRenderPipePreview(bool isActive)
    {
        connectorPartPreview.SetActive(isActive);
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (previewPipeParts[i] != null)
            {
                if (activatedPipes[i])
                {
                    //Display/undisplay every pipe which is activated
                    previewPipeParts[i].SetActive(isActive);

                    CubeInteraction neighborTile = neighbors[i].GetComponent<CubeInteraction>();

                    if (neighborTile.activatedPipes[GetOppositeFace(i)])
                        neighborTile.previewPipeParts[GetOppositeFace(i)].SetActive(isActive);
                }
            }
        }
    }

    static void OnPipeChanged(Changed<CubeInteraction> changed) // static because of networked var isPiped
    {
        Debug.Log($"{Time.time} OnPipeChanged value {changed.Behaviour.TileOccupied}");
        bool isPipedCurrent = changed.Behaviour.TileOccupied;

        //Load the old value of isPiped
        changed.LoadOld();

        changed.Behaviour.OnPipeRender(isPipedCurrent);
    }

    void OnPipeRender(bool isPipedCurrent)
    {
       if (isPipedCurrent)
            EnableTile();
    }
    private int GetOppositeFace(int i)
    {
        return i + 1 - 2 * (i % 2);            
    }
}
