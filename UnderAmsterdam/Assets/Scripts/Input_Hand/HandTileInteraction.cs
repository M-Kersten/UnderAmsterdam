using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host.Rig;

public class HandTileInteraction : NetworkBehaviour
{
    //Boolean to switch hand preferences
    public bool isRightHanded = true;

    public RigPart side;
    public NetworkRig rig;

    [SerializeField] private PlayerData myPlayer;
    [SerializeField] private bool TriggerPressed = false;
    [SerializeField] private HammerScript myHammerScript;

    private bool handEnabled = true;
    private void Start()
    {
        //Disable hands while the count down is happening
        Gamemanager.Instance.CountDownStart.AddListener(ToggleHands);
        Gamemanager.Instance.CountDownEnd.AddListener(ToggleHands);
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput<RigInput>(out var playerInputData)) //Get the input from the players 
        {
            if (side == RigPart.RightController)
            {
                TriggerPressed = playerInputData.rightTriggerPressed && isRightHanded;

                // Switch to the Hammer/Hand if the Grip is pressed
                myHammerScript.ActivateHammer(playerInputData.rightGripPressed && !isRightHanded);
            }

            if (side == RigPart.LeftController)
            {
                TriggerPressed = playerInputData.leftTriggerPressed && !isRightHanded;

                // Switch to the Hammer/Hand if the Grip is pressed
                myHammerScript.ActivateHammer(playerInputData.leftGripPressed && isRightHanded);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (handEnabled) {
        if (!rig.IsLocalNetworkRig)
            return;

            if (other.gameObject.layer == 7 && (side == RigPart.RightController && isRightHanded || side == RigPart.LeftController && !isRightHanded))
            {
                CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
                cubeScript.OnHandEnter(myPlayer.company);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (handEnabled)
        {
            if (!rig.IsLocalNetworkRig)
                return;

            if (other.gameObject.layer == 7)
            {
                CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
                cubeScript.OnHandExit(myPlayer.company);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 7 && TriggerPressed) // 7 is the layer for Tile
        {
            CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
            if(!cubeScript.playerInside && !cubeScript.TileOccupied)
            {
                cubeScript.UpdateCompany(myPlayer.company);
                cubeScript.EnableTile();
                TriggerPressed = false;
            }
        }
    }
    private void ToggleHands()
    {
        handEnabled = !handEnabled;
    }
}