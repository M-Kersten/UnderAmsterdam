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
                if (isRightHanded) TriggerPressed = playerInputData.rightHandCommand.triggerCommand > .5f;
                else
                {
                    // Switch to the Hammer/Hand if the Grip is pressed
                    if (!HammerEnabled && playerInputData.rightHandCommand.gripCommand > .5f)
                    {
                        myHammerScript.ActivateHammer(true);
                        HammerEnabled = true;
                    }
                    if (HammerEnabled && !(playerInputData.rightHandCommand.gripCommand > .5f))
                    {
                        myHammerScript.ActivateHammer(false);
                        HammerEnabled = false;
                    }
                }
            }

            if (side == RigPart.LeftController)
            {
                if (!isRightHanded) TriggerPressed = playerInputData.leftHandCommand.triggerCommand > .5f;
                else
                {
                    // Switch to the Hammer/Hand if the Grip is pressed
                    if (!HammerEnabled && playerInputData.leftHandCommand.gripCommand > .5f)
                    {
                        myHammerScript.ActivateHammer(true);
                        HammerEnabled = true;
                    }
                    if (HammerEnabled && !(playerInputData.leftHandCommand.gripCommand > .5f))
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
                    cubeScript.OnHandEnter(myPlayer.Company);
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
                    cubeScript.OnHandExit(myPlayer.Company);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (handEnabled && other.gameObject.layer == 7 && TriggerPressed) // 7 is the layer for Tile
        {
            CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
            if (!cubeScript.obstructed && !cubeScript.playerInside && !cubeScript.TileOccupied && cubeScript.VerifyRules(myPlayer.Company))
            {
                cubeScript.UpdateCompany(myPlayer.Company);
                if(HasStateAuthority)
                {
                    if (!cubeScript.isPlayTile)
                        Gamemanager.Instance.pManager.RemovePoints(myPlayer.Company);
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

    public void SwitchHands()
    {
        myHammerScript.ActivateHammer(false);
        HammerEnabled = false;

        isRightHanded = !isRightHanded;
    }
}