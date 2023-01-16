using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;

public class EnvironmentScript : NetworkBehaviour
{
    [SerializeField] private GameObject rock;

    private List<GameObject> addedObjects;

    private int lenght = 3, width = 4;

    private bool doneOnce = false;

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SpawnRock(int randomX, int randomY, int sizeX, int sizeY, int sizeZ, float rotationY)
    {
        GameObject spawned = Instantiate(rock, new Vector3(randomX, -0.625f, randomY), Quaternion.Euler(-90f, rotationY, 0f));
        spawned.transform.localScale = new Vector3(sizeX, sizeY, sizeZ);

        addedObjects.Add(spawned);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DestroyRock()
    {
        doneOnce = false;
        foreach (GameObject spawned in addedObjects)
            Destroy(spawned);
    }

    // Start is called before the first frame update
    void Start()
    {
        addedObjects = new List<GameObject>();
        Gamemanager.Instance.CountDownEnd.AddListener(spawnRock);
        Gamemanager.Instance.GameEnd.AddListener(RPC_DestroyRock);
    }

    void spawnRock()
    {
        if (!HasStateAuthority || doneOnce) return;

        doneOnce = true;
        for (int i = 0; i < Random.Range(1, 4); i++)
            RPC_SpawnRock(Random.Range(1 - lenght, lenght), Random.Range(-width, width), Random.Range(20, 60), Random.Range(20, 60), Random.Range(40, 80), (float)Random.Range(0, 360));
    }
}
