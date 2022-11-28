using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerScript : MonoBehaviour
{
    [SerializeField] PlayerData myData;

    private Vector3 prevPosition;
    private Vector3 deltaPos = Vector3.zero;

    private void Start()
    {
        prevPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7 && deltaPos.magnitude > 0.2f)
        {
            CubeInteraction touchedCube = other.GetComponent<CubeInteraction>();

            // Checks the company and the tile state
            if (touchedCube.TileOccupied && touchedCube.company == myData.company)
                touchedCube.DisableTile();
        }
    }

    private void SavePosition()
    {
        // Compute velocity of the Hammer whenever it is active
        deltaPos = (prevPosition - transform.position);
        prevPosition = transform.position;
    }

}