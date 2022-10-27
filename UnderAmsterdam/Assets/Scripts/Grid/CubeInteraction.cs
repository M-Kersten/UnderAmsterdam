using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.XR.Host;

public class CubeInteraction : NetworkBehaviour
{
    private enum Direction { Up, Down, Left, Right, Front, Behind };

    private bool showCenterPart = false;
    private GameObject nPipes;

    [SerializeField] private GameObject[] PipePreview, PipeHolder;
    [SerializeField] private NetworkObject[] neighbors;

    [SerializeField] private int company = 1;
    [SerializeField] private bool TileOccupied = false;
    [SerializeField] private bool isHover = false;

    public GameObject hPipes;
    public GameObject previewPipes;
    public bool[] activatedPipes;

    public override void Spawned()
    {
        activatedPipes = new bool[6];//Array of booleans storing which orientation is enabled [N, S, E, W, T, B]

        PipeHolder = hPipes.GetComponentsInChildren<GameObject>();
        PipePreview = previewPipes.GetComponentsInChildren<GameObject>();

        neighbors = new NetworkObject[6]; //Cubes have 6 faces, thus we will always need 6 neigbors
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

        showCenterPart = false;

        for (i = 0; i < 6; i++)
        {
            //Reset
            activatedPipes[i] = false;

            //Check all its nearby neighbors
            if (company == neighbors[i].GetComponent<CubeInteraction>().company)
            {
                pipeCount++;
                activatedPipes[i] = enable;
                neighbors[i].GetComponent<CubeInteraction>().activatedPipes[i + 1 - 2 * (i % 2)] = enable;
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
        for (int i = 0; i < 6; i++)
        {
            if (activatedPipes[i])
            {
                //Display/undisplay every pipe which is activated
                PipeHolder[i].SetActive(isActive);

                if (neighbors[i].GetComponent<CubeInteraction>().activatedPipes[i + 1 - 2 * (i % 2)])
                    neighbors[i].GetComponent<CubeInteraction>().PipeHolder[i + 1 - 2 * (i % 2)].SetActive(isActive);
            }
        }
    }

    private void OnRenderPipePreview(bool isActive)
    {
        for (int i = 0; i < 6; i++)
        {
            if (activatedPipes[i])
            {
                //Display/undisplay every pipe which is activated
                PipePreview[i].SetActive(isActive);

                if (neighbors[i].GetComponent<CubeInteraction>().activatedPipes[i + 1 - 2 * (i % 2)])
                    neighbors[i].GetComponent<CubeInteraction>().PipePreview[i + 1 - 2 * (i % 2)].SetActive(isActive);
            }
        }
    }
}
