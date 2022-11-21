using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerScript : MonoBehaviour
{
    [SerializeField] PlayerData myData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7) {
            CubeInteraction touchedCube = other.GetComponent<CubeInteraction>();
            if (touchedCube.TileOccupied && touchedCube.company == myData.company)
                touchedCube.DisableTile();
        }
    }
}
