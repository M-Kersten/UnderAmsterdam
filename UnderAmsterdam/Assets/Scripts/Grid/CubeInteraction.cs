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
    [SerializeField] private bool isHover = false;

    public GameObject lineEdges;
    public GameObject connector;
    public GameObject connectorPreview;
    public GameObject[] hPipes;
    public GameObject[] previewPipes;
    public bool[] activatedPipes;

    public override void Spawned()
    {
        hPipes = new GameObject[neighbors.Length];
        previewPipes = new GameObject[neighbors.Length];
        
        int i = 0;
        foreach (Transform pipe in PipeHolder)
            hPipes[i++] = pipe.GetComponent<GameObject>();
        i = 0;
        foreach (Transform pipe in PipePreview)
            previewPipes[i++] = pipe.GetComponent<GameObject>();

        activatedPipes = new bool[neighbors.Length];//Array of booleans storing which orientation is enabled [N, S, E, W, T, B]

        neighbors = new NetworkObject[neighbors.Length]; //Cubes have 6 faces, thus we will always need 6 neigbors
        GetNeighbors();
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
            isHover = true;
            NeighborCheck(true);
            OnRenderPipePreview(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!TileOccupied)
        {
            isHover = false;
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

            CubeInteraction cube = neighbors[i].GetComponent<CubeInteraction>();

            //Check all its nearby neighbors
            if (company == cube.company)
            {
                pipeCount++;
                activatedPipes[i] = enable;
                cube.activatedPipes[i + 1 - 2 * (i % 2)] = enable;
            }

            //Specific cases
            if (pipeCount < 2)
            {
                for (i = 0; pipeCount == 1 && activatedPipes[i] == false; i++);
                activatedPipes[i] = true;
                activatedPipes[i + 1 - 2 * (i % 2)] = true;

                return;
            }
        }
    }

    public void EnableTile()
    {
        OnRenderPipePreview(false);
        NeighborCheck(true);
        OnRenderPipe(true);
        TileOccupied = true;
        isHover = false;
    }

    private void OnRenderPipe(bool isActive)
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (activatedPipes[i])
            {
                //Display/undisplay every pipe which is activated
                hPipes[i].SetActive(isActive);

                CubeInteraction cube = neighbors[i].GetComponent<CubeInteraction>();

                if (cube.activatedPipes[i + 1 - 2 * (i % 2)])
                    cube.hPipes[i + 1 - 2 * (i % 2)].SetActive(isActive);
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
                previewPipes[i].SetActive(isActive);

                CubeInteraction cube = neighbors[i].GetComponent<CubeInteraction>();

                if (cube.activatedPipes[i + 1 - 2 * (i % 2)])
                    cube.previewPipes[i + 1 - 2 * (i % 2)].SetActive(isActive);
            }
        }
        connectorPreview.SetActive(isActive);
        lineEdges.SetActive(isActive);
    }
}
