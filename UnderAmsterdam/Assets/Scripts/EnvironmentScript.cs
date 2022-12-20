using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;

public class EnvironmentScript : NetworkBehaviour
{
    [SerializeField] private NetworkObject rock;

    private NetworkRunner runner;
    private List<NetworkObject> addedObjects;

    private int lenght = 3, width = 4;

    // Start is called before the first frame update
    void Start()
    {
        addedObjects = new List<NetworkObject>();
        runner = FindObjectOfType<NetworkRunner>();
        Gamemanager.Instance.GameStart.AddListener(spawnRock);
        //Gamemanager.Instance.GameEnd.AddListener(destroyRock);
    }

    private void Update()
    {
        if (Input.GetKeyDown("z"))
            destroyRock();
    }

    void spawnRock()
    {
        for (int i = 0; i < Random.Range(1,4); i++)
        {
            int randomX = Random.Range(1 - lenght, lenght);
            int randomY = Random.Range(-width, width);

            rock.gameObject.transform.localScale = new Vector3(Random.Range(20, 60), Random.Range(20, 60), Random.Range(40, 80));
            NetworkObject spawned = runner.Spawn(rock, new Vector3(randomX, 0.15f, randomY), Quaternion.Euler(-90f, (float)Random.Range(0, 360), 0f));

            addedObjects.Add(spawned);
        }
    }

    void destroyRock()
    {
        foreach (NetworkObject spawned in addedObjects)
        {
            runner.Despawn(spawned);
        }
    }
}
