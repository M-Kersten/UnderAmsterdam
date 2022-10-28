using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public struct PlayerInputData : INetworkInput
{
    public NetworkBool anyTriggerPressed;
    public NetworkBool rightTriggerPressed;
    public NetworkBool leftTriggerPressed;
    public NetworkBool anyGripPressed;
    public NetworkBool rightGripPressed;
    public NetworkBool leftGripPressed;
}
