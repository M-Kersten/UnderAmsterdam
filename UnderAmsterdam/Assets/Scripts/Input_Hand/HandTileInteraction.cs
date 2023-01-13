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

    private AudioSource audioSource;
    [SerializeField] private AudioClip placingPipe1, placingPipe2, placingPipe3;

    [SerializeField] private PlayerData myPlayer;
    [SerializeField] private bool TriggerPressed = false;
    [SerializeField] private HammerScript myHammerScript;

    public bool isRightHanded = true;
    private bool handEnabled = true;
    private bool HammerEnabled = false;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //Disable hands while the count down is happening
        Gamemanager.Instance.CountDownStart.AddListener(ToggleHands);
        Gamemanager.Instance.CountDownEnd.AddListener(ToggleHands);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_EnableTile(CubeInteraction cubeScript)
    {
        // Plays random block placing sound
        int randomSound = Random.Range(0, 3);
        switch (randomSound)
        {
            case 0:
                audioSource.PlayOneShot(placingPipe1);
                break;
            case 1:
                audioSource.PlayOneShot(placingPipe2);
                break;
            case 2:
                audioSource.PlayOneShot(placingPipe3);
                break;
        }

        cubeScript.EnableTile();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (GetInput<RigInput>(out var playerInputData)) //Get the input from the players 
        {
            if (side == RigPart.RightController)
            {
                if (isRightHanded) TriggerPressed = playerInputData.rightTriggerPressed;
                else
                {
                    // Switch to the Hammer/Hand if the Grip is pressed
                    if (!HammerEnabled && playerInputData.rightGripPressed)
                    {
                        myHammerScript.ActivateHammer(true);
                        HammerEnabled = true;
                    }
                    if (HammerEnabled && !playerInputData.rightGripPressed)
                    {
                        myHammerScript.ActivateHammer(false);
                        HammerEnabled = false;
                    }
                }
            }

            if (side == RigPart.LeftController)
            {
                if (!isRightHanded) TriggerPressed = playerInputData.leftTriggerPressed;
                else
                {
                    // Switch to the Hammer/Hand if the Grip is pressed
                    if (!HammerEnabled && playerInputData.leftGripPressed)
                    {
                        myHammerScript.ActivateHammer(true);
                        HammerEnabled = true;
                    }
                    if (HammerEnabled && !playerInputData.leftGripPressed)
                    {
                        myHammerScript.ActivateHammer(false);
                        HammerEnabled = false;
                    }
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (handEnabled)
        {
            if (!rig.IsLocalNetworkRig)
                return;

            if (other.gameObject.layer == 7 && (side == RigPart.RightController && isRightHanded || side == RigPart.LeftController && !isRightHanded))
            {
                CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
                if (cubeScript)
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

            if (other.gameObject.layer == 7 && (side == RigPart.RightController && isRightHanded || side == RigPart.LeftController && !isRightHanded))
            {
                CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
                if (cubeScript)
                    cubeScript.OnHandExit(myPlayer.company);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (handEnabled && other.gameObject.layer == 7 && TriggerPressed) // 7 is the layer for Tile
        {
            CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
            if (!cubeScript.obstructed && !cubeScript.playerInside && !cubeScript.TileOccupied && cubeScript.VerifyRules(myPlayer.company))
            {
                cubeScript.UpdateCompany(myPlayer.company);
                if(HasStateAuthority)
                {
                    Gamemanager.Instance.pManager.RemovePoints(myPlayer.company);
                    RPC_EnableTile(cubeScript);
                }
                TriggerPressed = false;
            }
        }
    }
    private void ToggleHands()
    {
        handEnabled = !handEnabled;
    }
}