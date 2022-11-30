using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class HammerScript : NetworkBehaviour
{
    [SerializeField] PlayerData myData;

    private Vector3 prevPosition;
    private Vector3 deltaPos = Vector3.zero;

    [Networked(OnChanged = nameof(OnHammerChange))]
    public bool isActive { get; set; }

    private void Start()
    {
        prevPosition = transform.position;
        InvokeRepeating("SavePosition", 0f, 0.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7 && deltaPos.magnitude > 0.15f)
        {
            CubeInteraction touchedCube = other.GetComponent<CubeInteraction>();

            // Checks the company and the tile state
            if (touchedCube.TileOccupied && touchedCube.company == myData.company)
                touchedCube.DisableTile();
        }
    }

    static void OnHammerChange(Changed<HammerScript> changed)
    {
        changed.Behaviour.gameObject.SetActive(changed.Behaviour.isActive);
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