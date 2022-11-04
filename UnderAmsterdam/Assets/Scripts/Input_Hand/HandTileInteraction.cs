using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;

public class HandTileInteraction : NetworkBehaviour
{
    public RigPart side;
    public NetworkRig rig;

    [SerializeField]
    private bool TriggerPressed = false;

    [Header("Ray Beamer")]
    public RayBeamer beamer;

    private Collider last;

    private void Awake()
    {
           //beamer.onRelease.AddListener(OnBeamRelease);
           beamer.onRayHit.AddListener(onRayHit);
           beamer.onRayExit.AddListener(onRayExit);
    }
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

/*    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 7 && TriggerPressed) // 7 is the layer for Tile
        {
            other.gameObject.GetComponent<CubeInteraction>().EnableTile();
            TriggerPressed = false;
            Debug.Log("Send trigger command to tile");
        }

    }*/

    protected virtual void onRayHit(Collider lastHitCollider, Vector3 position)
    {
        Debug.Log("OnRayHit");
        if (!lastHitCollider)
            return;
        last = lastHitCollider;
        if (lastHitCollider.gameObject.layer == 7)
        {
            lastHitCollider.gameObject.GetComponent<CubeInteraction>().OnRenderPipePreview(true);
            lastHitCollider.gameObject.GetComponent<CubeInteraction>().UpdateNeighborData(true);
        }

        if (lastHitCollider.gameObject.layer == 7 && TriggerPressed) // 7 is the layer for Tile
        {
            lastHitCollider.gameObject.GetComponent<CubeInteraction>().EnableTile();
            TriggerPressed = false;
            Debug.Log("Send trigger command to tile");
        }
    }

    protected virtual void onRayExit(Collider lastHitCollider, Vector3 lastHitPos)
    {
        if (!last)
            return;
        last.gameObject.GetComponent<CubeInteraction>().OnRenderPipePreview(false);
        last.gameObject.GetComponent<CubeInteraction>().UpdateNeighborData(false);
    }
}