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

    [SerializeField] private int company = 1;

    [Networked(OnChanged = nameof(OnPipeChanged))]
    public bool TileOccupied { get; set; } // can be changed and send over the network only by the host

    [SerializeField] private GameObject[] pipeParts;
    [SerializeField] private GameObject[] previewPipeParts;
    [SerializeField] private bool[] activatedPipes;

    private int amountFaces = 6;

    public bool isHover = false;

    [SerializeField] private BoxCollider[] colliderr;

    public override void Spawned()
    {
        OnRenderPipePreview(false);
        OnRenderPipePart(false);

        neighbors = new NetworkObject[amountFaces]; //Cubes have 6 faces, thus we will always need 6 neigbors
        GetNeighbors();

        StartCoroutine(ColliderDisable(1f));

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

        Ray ray = new Ray();
        ray.origin = transform.position;
        LayerMask layerMask = LayerMask.GetMask("Tile");

        ray.direction = Vector3.up;
        Debug.DrawRay(transform.position, Vector3.up, Color.green, 500f);
        if (Physics.Raycast(ray, out hit, 4f, layerMask))
            neighbors[(int)Direction.Up] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Up] = null;

        ray.direction = -Vector3.up;
        if (Physics.Raycast(ray, out hit, 4f, layerMask))
            neighbors[(int)Direction.Down] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Down] = null;

        ray.direction = Vector3.left;
        if (Physics.Raycast(ray, out hit, 4f, layerMask))
            neighbors[(int)Direction.Left] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Left] = null;

        ray.direction = Vector3.right;
        if (Physics.Raycast(ray, out hit, 4f, layerMask))
            neighbors[(int)Direction.Right] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Right] = null;

        ray.direction = Vector3.forward;
        if (Physics.Raycast(ray, out hit, 4f, layerMask))
            neighbors[(int)Direction.Front] = hit.transform.gameObject.GetComponent<NetworkObject>();
        else
            neighbors[(int)Direction.Front] = null;

        ray.direction = Vector3.back;
        if (Physics.Raycast(ray, out hit, 4f, layerMask))
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

    public void UpdateNeighborData(bool enable)
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] != null) {

                CubeInteraction neighborTile = neighbors[i].GetComponent<CubeInteraction>();

                if (neighborTile.company != 0 && (neighborTile.company == company || isHover == enable))
                {
                    activatedPipes[i] = enable;
                    neighborTile.activatedPipes[GetOppositeFace(i)] = enable;
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
        for(int i = 0; i < 6; ++i)
        {
            if(colliderr[i])colliderr[i].enabled = true;
        }
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

    public void OnRenderPipePreview(bool isActive)
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

    IEnumerator ColliderDisable(float time)
    {
        yield return new WaitForSeconds(time);

        colliderr = new BoxCollider[6];
        for (int j = 0; j < 6; ++j)
        {
            while (neighbors[j] && colliderr[j] == null)
                colliderr[j] = neighbors[j].gameObject.GetComponent<BoxCollider>();
            if (colliderr[j] && colliderr[j].name == "TileCorner")
                colliderr[j].enabled = true;
            else if (colliderr[j])
                colliderr[j].enabled = false;
        }
    }

}
