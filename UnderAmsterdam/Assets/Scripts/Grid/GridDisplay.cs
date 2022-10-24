using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host.Rig;

public class GridDisplay : MonoBehaviour
{
    //public NetworkObject block;
    public GameObject cube;
    public Transform parent;

    public Vector3 StartPos;
    public uint width = 8;
    public uint height = 4;
    public uint depth = 8;
    public GameObject[,,] GridA; //Array with all the cubes
    //public NetworkObject[,,] GridA; //Array with all the cubes

    private protected uint nbPipes;

    private void Start()
    { 
        GridA = new GameObject[width, height, depth];
        //GridA = new NetworkObject[width, height, depth];
        for (uint x = 0; x < width; ++x)
        {
            for (uint y = 0; y < height; ++y)
            {
                for (uint z = 0; z < depth; ++z)
                {
                    //GridA[x, y, z] = runner.Spawn(cube, new Vector3(StartPos.x + x * 0.5f, StartPos.y + y * 0.5f, 0.5f * z + StartPos.z), Quaternion.identity);
                    GridA[x, y, z] = Instantiate(cube, new Vector3(StartPos.x + x * 0.5f, StartPos.y + y * 0.5f, 0.5f * z + StartPos.z), Quaternion.identity, parent);
                    GridA[x, y, z].transform.name = "Cube: " + x + ' ' + y + ' ' + z;
                    ++nbPipes;
                }
            }
        }

    }
}
