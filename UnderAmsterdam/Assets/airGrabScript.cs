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

    [SerializeField] private GameObject worldObject;

    private void Start()
    {
        onTriggerDown = new UnityEvent();
        onTriggerUp = new UnityEvent();

        //handSide = RigPart.LeftController;
        //button.EnableWithDefaultXRBindings(side: handSide, new List<string> { "thumbstickClicked", "primaryButton", "secondaryButton" });

        handSide = RigPart.RightController;
        button.EnableWithDefaultXRBindings(side: handSide, new List<string> { "thumbstickClicked", "primaryButton", "secondaryButton" });

        onTriggerDown.AddListener(TriggerDownOnce);
        onTriggerUp.AddListener(TriggerUpOnce);
    }

    private void FixedUpdate()
    {
        triggerPressed = button.action.ReadValue<float>() >= 0.5f;

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
            Vector3 transformation = anchorPoint - transform.position;
            worldObject.transform.position += transformation;
            anchorPoint = transform.position;
        }
    }

    private void TriggerDownOnce()
    {
        anchorPoint = transform.position;
    }
    private void TriggerUpOnce()
    {
        anchorPoint = Vector3.zero;
    }
}
