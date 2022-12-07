using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host.Rig;
using UnityEngine.Serialization;

public class HandTileInteraction : NetworkBehaviour
{
    public RigPart side;
    public NetworkRig rig;

    [SerializeField] private PlayerData myPlayer;
    [SerializeField] private SkinnedMeshRenderer handRenderer;
    [SerializeField] private Material[] handMaterial;

    [SerializeField] private bool triggerPressed = false;

    private string companyOld;
    private Dictionary<string, Material> handDic;
    private bool isSpawned = false;
    public override void Spawned() 
    {
        handDic = new Dictionary<string, Material>()
        {
            {"", handMaterial[0]},
            {"none", handMaterial[0]},
            {"sewage", handMaterial[0]},
            {"data", handMaterial[1]},
            {"gas", handMaterial[2]},
            {"power", handMaterial[3]},
            {"water", handMaterial[4]}
        };
        isSpawned = true;
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(!isSpawned)
            return;
        if (GetInput<RigInput>(out var playerInputData)) //Get the input from the players 
        {
            if(side == RigPart.RightController)
                triggerPressed = playerInputData.rightTriggerPressed;
            
            if(side == RigPart.LeftController)
                triggerPressed = playerInputData.leftTriggerPressed;
        }
        
        if (myPlayer.company != companyOld && handDic.ContainsKey(myPlayer.company)) 
        {
            handRenderer.material = handDic[myPlayer.company];
            companyOld = myPlayer.company;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!rig.IsLocalNetworkRig)
            return;

        if (other.gameObject.layer == 7)
        {
            CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
            cubeScript.OnHandEnter(myPlayer.company);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!rig.IsLocalNetworkRig)
            return;

        if (other.gameObject.layer == 7)
        {
            CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
            cubeScript.OnHandExit(myPlayer.company);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 7 && triggerPressed) // 7 is the layer for Tile
        {
            CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
            if(!cubeScript.TileOccupied)
            {
                cubeScript.UpdateCompany(myPlayer.company);
                cubeScript.EnableTile();
                triggerPressed = false;
            }
        }
    }
}