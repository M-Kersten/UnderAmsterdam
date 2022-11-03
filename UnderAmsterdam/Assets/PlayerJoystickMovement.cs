using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerJoystickMovement : MonoBehaviour
{
    PlayerInputHandler localPlayerInput;
    private void Start()
    {
        localPlayerInput = GetComponent<PlayerInputHandler>();
    }
    private void FixedUpdate()
    {
        Debug.Log("Left hand: " + localPlayerInput.leftjoystickPosition);
        Debug.Log("Right hand: " + localPlayerInput.rightjoystickPosition);
    }
}
