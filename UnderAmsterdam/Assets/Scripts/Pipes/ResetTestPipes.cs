using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTestPipes : MonoBehaviour
{
    [SerializeField] private List<CubeInteraction> tiles;
    public void ResetPlayArea()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if(tiles[i].TileOccupied)
                tiles[i].DisableTile();
        }
    }
}