using UnityEngine;
using Fusion;

public class HammerScript : NetworkBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip destroyingPipe1, destroyingPipe2, destroyingPipe3;

    [SerializeField] PlayerData myData;
    [SerializeField] private GameObject myHammer;

    private Vector3 prevPosition;
    private Vector3 deltaPos = Vector3.zero;

    private ChangeDetector _changes;

    [Networked]
    public bool isActive { get; set; }
    
    private void OnHammerChange(bool previousState, bool newState)
    {
        ActivateHammer(newState);
    }

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(OnHammerChange):
                    var reader = GetPropertyReader<bool>(nameof(OnHammerChange));
                    var (previous,current) = reader.Read(previousBuffer, currentBuffer);
                    OnHammerChange(previous, current);
                    break;
            }
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        prevPosition = transform.position;
        InvokeRepeating("SavePosition", 0f, 0.1f);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DisableTile(CubeInteraction touchedCube)
    {
        // Plays random block destroying sound
        int randomSound = Random.Range(0, 3);
        switch (randomSound)
        {
            case 0:
                audioSource.PlayOneShot(destroyingPipe1);
                break;
            case 1:
                audioSource.PlayOneShot(destroyingPipe2);
                break;
            case 2:
                audioSource.PlayOneShot(destroyingPipe3);
                break;
        }

        touchedCube.DisableTile();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DisableRoot(NetworkObject touchedRoot)
    {
        // Plays random block destroying sound
        int randomSound = Random.Range(0, 3);
        switch (randomSound)
        {
            case 0:
                audioSource.PlayOneShot(destroyingPipe1);
                break;
            case 1:
                audioSource.PlayOneShot(destroyingPipe2);
                break;
            case 2:
                audioSource.PlayOneShot(destroyingPipe3);
                break;
        }
        touchedRoot.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7 && deltaPos.magnitude > 0.15f)
        {
            CubeInteraction touchedCube = other.GetComponent<CubeInteraction>();

            // Checks the company and the tile state
            if (touchedCube.TileOccupied && touchedCube.Company == myData.Company && HasStateAuthority)
            {
                RPC_DisableTile(touchedCube);
                if(!touchedCube.isPlayTile)
                    Gamemanager.Instance.pManager.AddPoints(myData.Company);
            }
        }
        
        //Disable treeRoots
        if (other.gameObject.layer == 14 && deltaPos.magnitude > 0.15f && HasStateAuthority)
        {
            NetworkObject root = other.gameObject.GetComponentInParent<NetworkObject>();
            RPC_DisableRoot(root);
            Gamemanager.Instance.pManager.RemovePointsRoots(myData.Company);
        }
    }
    
    public void ActivateHammer(bool enable)
    {
        isActive = enable;
        myHammer.SetActive(enable);
    }

    void SavePosition()
    {
        if (isActive)
        {
            // Compute velocity of the Hammer whenever it is active
            deltaPos = (prevPosition - transform.position);
            prevPosition = transform.position;
        }
    }
}