using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class airGrabScript : MonoBehaviour
{
    public RigPart handSide;
    public InputActionProperty button;

    private UnityEvent onTriggerDown;
    private UnityEvent onTriggerUp;

    private Vector3 anchorPoint;

    [SerializeField] private bool triggerPressed = false;
    [SerializeField] private bool triggerIsActive = false;

    private void Start()
    {
        handSide = RigPart.LeftController;
        button.EnableWithDefaultXRBindings(side: handSide, new List<string> { "primary" });

        handSide = RigPart.RightController;
        button.EnableWithDefaultXRBindings(side: handSide, new List<string> { "primary" });

        onTriggerDown.AddListener(TriggerDownOnce);
        onTriggerUp.AddListener(TriggerUpOnce);
    }

    private void FixedUpdate()
    {
        triggerPressed = button.action.triggered;

        if (triggerPressed && !triggerIsActive)
        {
            onTriggerDown.Invoke();
            triggerIsActive = true;
        } else if (!triggerPressed && triggerIsActive)
        {
            onTriggerUp.Invoke();
            triggerIsActive = false;
        }

        if (triggerPressed)
        {
            //Do code that moves player
        }
    }

    private void TriggerDownOnce()
    {
        anchorPoint = gameObject.transform.position;
    }
    private void TriggerUpOnce()
    {
        anchorPoint = Vector3.zero;
    }
}
