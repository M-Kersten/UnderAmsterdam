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

    public override void Spawned()
    {
    }

    public override void FixedUpdateNetwork()
    {
        if (rig.isActiveAndEnabled && !rig.IsLocalNetworkRig)
            this.enabled = false;

        base.FixedUpdateNetwork();
        if (GetInput<PlayerInputData>(out PlayerInputData playerInputData))
        {
            //Debug.Log("Get");
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