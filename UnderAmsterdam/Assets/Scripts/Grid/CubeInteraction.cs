using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.XR.Host;

public class CubeInteraction : NetworkBehaviour
{
    private enum Direction { Up, Down, Left, Right, Front, Behind };

    [SerializeField] private Transform PipePreview, PipeHolder;
    [SerializeField] private NetworkObject[] neighbors;

    [SerializeField] private int company = 1;
    [SerializeField] private bool TileOccupied = false;

    [SerializeField] private GameObject lineEdges;
    [SerializeField] private GameObject connector;
    [SerializeField] private GameObject connectorPreview;
    
    private GameObject[] pipeParts;
    private GameObject[] previewPipeParts;
    private bool[] activatedPipes;

    private int amountFaces = 6;

    public override void Spawned()
    {
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
            NeighborCheck(true);
            OnRenderPipePreview(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!TileOccupied)
        {
            NeighborCheck(false);
            OnRenderPipePreview(false);
        }
    }

    private void NeighborCheck(bool enable)
    {
        int pipeCount = 0;
        int i;

        for (i = 0; i < neighbors.Length; i++)
        {
            //Reset
            activatedPipes[i] = false;

            CubeInteraction neighborTile = neighbors[i].GetComponent<CubeInteraction>();

            //Check all its nearby neighbors
            if (company == neighborTile.company)
            {
                pipeCount++;
                activatedPipes[i] = enable;
                neighborTile.activatedPipes[GetOppositeFace(i)] = enable;
            }

            //Specific cases
            if (pipeCount < 2)
            {
                for (i = 0; pipeCount == 1 && activatedPipes[i] == false; i++);
                activatedPipes[i] = true;
                activatedPipes[GetOppositeFace(i)] = true;

                return;
            }
        }
    }

    public void EnableTile()
    {
        OnRenderPipePreview(false);
        NeighborCheck(true);
        OnRenderPipePart(true);
        TileOccupied = true;
    }

    private void OnRenderPipePart(bool isActive)
    {
        for (int i = 0; i < neighbors.Length; i++)
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
        connector.SetActive(isActive);
    }

    private void OnRenderPipePreview(bool isActive)
    {
        for (int i = 0; i < neighbors.Length; i++)
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
        connectorPreview.SetActive(isActive);
        lineEdges.SetActive(isActive);
    }

    private int GetOppositeFace(int i)
    {
        return i + 1 - 2 * (i % 2);
    }
}
