using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;
using UnityEngine.InputSystem;

public class airGrabScript : MonoBehaviour
{
    public RigPart hand;
    public InputActionProperty button;

    [SerializeField]
    private bool TriggerPressed = false;

    private void FixedUpdate()
    {
        TriggerPressed = button.action.triggered;

        if (TriggerPressed)
        {

        }
    }
}
