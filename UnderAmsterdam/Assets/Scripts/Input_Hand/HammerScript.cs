using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerScript : MonoBehaviour
{
    [SerializeField] PlayerData myData;

    private Vector3 prevPosition;
    private Vector3 velocity = Vector3.zero;

    public bool isActive = false;

    private void Start()
    {
        prevPosition = transform.position;
        InvokeRepeating("ComputeVelocity", 0f, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7 && velocity.magnitude > 1.25f) {
            CubeInteraction touchedCube = other.GetComponent<CubeInteraction>();
            if (touchedCube.TileOccupied && touchedCube.company == myData.company)
                touchedCube.DisableTile();
        }
    }

    private void ComputeVelocity()
    {
        if (isActive)
        {
            velocity = (prevPosition - transform.position) / 0.1f;
            prevPosition = transform.position;
        }
    }

}
