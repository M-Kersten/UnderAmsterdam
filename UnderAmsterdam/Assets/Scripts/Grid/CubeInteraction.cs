using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.XR.Host;

public class CubeInteraction : NetworkBehaviour
{
    private enum Direction { Up, Down, Left, Right, Front, Behind };

    [SerializeField] private GameObject PipePreview, PipeHolder;
    [SerializeField] private NetworkObject[] neighbors;

    [SerializeField] private bool TileOccupied;
    [SerializeField] private bool isHover = false;

    public override void Spawned()
    {
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
            OnRenderPipePreview(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!TileOccupied)
        {
            OnRenderPipePreview(false);
            isHover = false;
        }
    }

    public void EnableTile()
    {
        OnRenderPipePreview(false);
        OnRenderPipe(true);
        isHover = false;
    }

    private void OnRenderPipe(bool isActive)
    {
        PipeHolder.SetActive(isActive);
    }
    private void OnRenderPipePreview(bool isActive)
    {
        PipePreview.SetActive(isActive);
    }
}
