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
    private float timeLeft;

    private void Awake()
    {
           beamer.onRayHit.AddListener(onRayHit);
           beamer.onRayExit.AddListener(onRayExit);
    }
    private void Update()
    {
        timeLeft -= Time.deltaTime;
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 7 && other.gameObject.GetComponent<CubeInteraction>().modLaser == false && TriggerPressed) // 7 is the layer for Tile
        {
            string company = transform.parent.transform.parent.gameObject.GetComponent<PlayerData>().company;
            CubeInteraction cubeScript = other.gameObject.GetComponent<CubeInteraction>();
            cubeScript.UpdateCompany(company);
            cubeScript.GetComponent<CubeInteraction>().EnableTile();
            TriggerPressed = false;
            Debug.Log("Send trigger command to tile");
        }

    }

    protected virtual void onRayHit(Collider lastHitCollider, Vector3 position)
    {
        Debug.Log("OnRayHit");
        if (!lastHitCollider)
            return;


        if (lastHitCollider.gameObject.layer == 7)
        {
            lastHitCollider.gameObject.GetComponent<CubeInteraction>().OnRenderPipePreview(true);
            lastHitCollider.gameObject.GetComponent<CubeInteraction>().UpdateNeighborData(true);
            last = lastHitCollider;
        }

        if (lastHitCollider.gameObject.layer == 7 && TriggerPressed && timeLeft <= 0) // 7 is the layer for Tile
        {
            string company = transform.parent.transform.parent.gameObject.GetComponent<PlayerData>().company;
            CubeInteraction cubeScript = lastHitCollider.gameObject.GetComponent<CubeInteraction>();
            cubeScript.UpdateCompany(company);
            cubeScript.GetComponent<CubeInteraction>().EnableTile();
            TriggerPressed = false;
            timeLeft = 0.6f;
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