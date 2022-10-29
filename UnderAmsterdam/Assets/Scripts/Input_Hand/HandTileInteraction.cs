using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host.Rig;

public class HandTileInteraction : NetworkBehaviour
{
    public RigPart side;
    public NetworkRig rig;

    [SerializeField]
    private bool TriggerPressed = false;
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput<RigInput>(out var playerInputData)) //Get the input from the players 
        {
            if(side == RigPart.RightController)
                TriggerPressed = playerInputData.rightTriggerPressed;
            
            if(side == RigPart.LeftController)
                TriggerPressed = playerInputData.leftTriggerPressed;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Tile") && TriggerPressed)
        {
            other.gameObject.GetComponent<CubeInteraction>().EnableTile();
            TriggerPressed = false;
        }

    }

}