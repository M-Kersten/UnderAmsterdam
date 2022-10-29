using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;

public class CubeInteraction : NetworkBehaviour
{
    private enum Direction { Up, Down, Left, Right, Front, Behind };

    [SerializeField] private GameObject PipePreview, PipeHolder;
    [SerializeField] private NetworkObject[] neighbors;


    [Networked(OnChanged = nameof(OnPipeChanged))] 
    public bool TileOccupied { get; set; } // can be changed and send over the network only by the host

    private bool isSpawned = false;

    // TEST PART //
    private NetworkRunner runner;

    public override void Spawned()
    {
        OnRenderPipePreview(false);
        OnRenderPipe(false);

        neighbors = new NetworkObject[6]; //Cubes have 6 faces, thus we will always need 6 neigbors
        GetNeighbors();

        isSpawned = true;
        runner = this.GetComponent<NetworkObject>().Runner;
        this.GetComponent<NetworkObject>().AssignInputAuthority(runner.LocalPlayer);
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
        if (isSpawned && !TileOccupied)
            OnRenderPipePreview(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (isSpawned && !TileOccupied)
            OnRenderPipePreview(false);
    }

    public void EnableTile()
    {
        if (TileOccupied)
            return;

        OnRenderPipePreview(false);
        OnRenderPipe(true);
        TileOccupied = true;
    }

    private void OnRenderPipe(bool isActive)
    {
        PipeHolder.SetActive(isActive);
    }
    private void OnRenderPipePreview(bool isActive)
    {
        PipePreview.SetActive(isActive);
    }

    static void OnPipeChanged(Changed<CubeInteraction> changed) // static because of networked var isPiped
    {
        Debug.Log($"{Time.time} OnPipeChanged value {changed.Behaviour.TileOccupied}");
        bool isPipedCurrent = changed.Behaviour.TileOccupied;

        //Load the old value of isPiped
        changed.LoadOld();

        bool isPipedPrevious = changed.Behaviour.TileOccupied;

        //if (isPipedCurrent && !isPipedPrevious)
            changed.Behaviour.OnPipeRender(isPipedCurrent);
    }

    void OnPipeRender(bool isPipedCurrent)
    {
        if (isPipedCurrent)
            EnableTile();
    }
}
