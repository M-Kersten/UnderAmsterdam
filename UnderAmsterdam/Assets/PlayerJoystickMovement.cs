using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerJoystickMovement : NetworkBehaviour
{
    private void FixedUpdate()
    {
        if (GetInput<RigInput>(out var playerInputData))
        {
            Debug.Log("Left hand: " + playerInputData.leftJoystickPositon);
            Debug.Log("Right hand: " + playerInputData.rightJoystickPositon);
        }
    }
}
