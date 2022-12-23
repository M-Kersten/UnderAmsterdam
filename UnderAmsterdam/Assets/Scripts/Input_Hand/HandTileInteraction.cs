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

    private bool handEnabled = true;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

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
                TriggerPressed = playerInputData.rightTriggerPressed;

            if (side == RigPart.LeftController)
            {
                TriggerPressed = playerInputData.leftTriggerPressed;

                // Switch to the Hammer/Hand if the Grip is pressed
                myHammerScript.ActivateHammer(playerInputData.leftGripPressed);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (handEnabled)
        {
            if (!rig.IsLocalNetworkRig)
                return;

            if (other.gameObject.layer == 7)
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

            if (other.gameObject.layer == 7)
            {
                CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
                if (cubeScript)
                    cubeScript.OnHandExit(myPlayer.company);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 7 && TriggerPressed) // 7 is the layer for Tile
        {
            CubeInteraction cubeScript = other.GetComponent<CubeInteraction>();
            if (!cubeScript.obstructed && !cubeScript.playerInside && !cubeScript.TileOccupied && cubeScript.VerifyRules(myPlayer.company))
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

                Gamemanager.Instance.pManager.RemovePoints(myPlayer.company);
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