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
    [SerializeField] public NetworkObject[] neighbors;

    [SerializeField] public int company = 0;

    [Networked(OnChanged = nameof(OnPipeChanged))]
    public bool TileOccupied { get; set; } // can be changed and send over the network only by the host

    [SerializeField] private GameObject[] pipeParts;
    [SerializeField] private GameObject[] previewPipeParts;
    [SerializeField] private bool[] activatedPipes;

    private int CUBE_FACES_NB = 6;

    public bool isHover = false;
    public bool isChecked = false;

    public override void Spawned()
    {
        // Hides the tiles
        OnRenderPipePreview(false);
        OnRenderPipePart(false);

        neighbors = new NetworkObject[CUBE_FACES_NB]; //Cubes have 6 faces, thus we will always need 6 neigbors
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

    // Gets the neighbors tiles in all 6 directions
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

    public void checkWin()
    {
        // For each neighbor...
        for (int i = 0; i < neighbors.Length; i++)
        {
            // if it's a normal tile...
            if (neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
            {
                // from the same company and not checked yet...
                if (company == neighborTile.company && !neighborTile.isChecked)
                {
                    // Verify its neighbor and mark it as checked.
                    neighborTile.checkWin();
                    isChecked = true;
                }
            }
            // if it's an Output tile...
            else if (neighbors[i].TryGetComponent(out IOTileData inOutPut))
            {
                // from the same company and active...
                if (company == inOutPut.company && inOutPut.isActive)
                inOutPut.winGameEvent(); // Success !
                return;
            }
        }
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

    // Check all of its neighbors and activate the corresponding pipes if it's from the same company
    private void UpdateNeighborData(bool enable)
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null)
            {
                // Gets the neighbor tile
                if (neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
                {
                    // If the neighbor is in the same company...
                    if (neighborTile.company != 0 && (neighborTile.company == company || isHover == enable))
                    {
                        // activates the pipe facing the neighbor as well as the neighbor's pipe facing the current tile.
                        activatedPipes[i] = enable;
                        neighborTile.activatedPipes[GetOppositeFace(i)] = enable;
                    }
                }
                // Or the IO tile
                else if (neighbors[i].TryGetComponent(out IOTileData IOTile))
                {
                    if (IOTile.company != 0 && (IOTile.company == company || isHover == enable))
                        activatedPipes[i] = enable;
                }
            }
        }
    }

    public void EnableTile()
    {
        if (TileOccupied)
            return;

        isHover = false;
        TileOccupied = true;
        company = 1;
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
                    
                    if (neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
                    {
                        if (neighborTile.activatedPipes[GetOppositeFace(i)])
                            neighborTile.pipeParts[GetOppositeFace(i)].SetActive(isActive);
                    }
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

                    if (neighbors[i].TryGetComponent(out CubeInteraction neighborTile))
                    {
                        if (neighborTile.activatedPipes[GetOppositeFace(i)])
                            neighborTile.previewPipeParts[GetOppositeFace(i)].SetActive(isActive);
                    }
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
        // Getting the index of the opposite direction to i
        return i + 1 - 2 * (i % 2);            
    }
}
