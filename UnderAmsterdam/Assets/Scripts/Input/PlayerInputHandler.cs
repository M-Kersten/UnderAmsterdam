using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;

public class PlayerInputHandler : MonoBehaviour
{
    public InputActionProperty triggerActionL;
    public InputActionProperty triggerActionR;
    public InputActionProperty gripActionL;
    public InputActionProperty gripActionR;

    public NetworkRig networkRig;
    private RigPart side;

    [SerializeField]
    private bool isAnyTriggerPressed;
    [SerializeField]
    private bool isRightTriggerPressed;
    [SerializeField]
    private bool isLeftTriggerPressed;

    [SerializeField]
    private bool isAnyGripPressed;
    [SerializeField]
    private bool isRightGripPressed;    
    [SerializeField]
    private bool isLeftGripPressed;


    private void Awake()
    {
        //networkRig = GetComponent<NetworkRig>();

        side = RigPart.LeftController;
        triggerActionL.EnableWithDefaultXRBindings(side: side, new List<string> { "trigger" });
        gripActionL.EnableWithDefaultXRBindings(side: side, new List<string> { "grip" });

        side = RigPart.RightController;
        triggerActionR.EnableWithDefaultXRBindings(side: side, new List<string> { "trigger" });
        gripActionR.EnableWithDefaultXRBindings(side: side, new List<string> { "grip" });

    }

    private void Update()
    {
        if (networkRig.isActiveAndEnabled && !networkRig.IsLocalNetworkRig)
            this.enabled = false;

        /********************* Trigger *********************/

        if (triggerActionL.action.ReadValue<float>() >= 0.9f)
            isLeftTriggerPressed = true;
        else
            isLeftTriggerPressed = false;
        
        if (triggerActionR.action.ReadValue<float>() >= 0.9f)
            isRightTriggerPressed = true;
        else
            isRightTriggerPressed = false;

        if (isLeftTriggerPressed || isRightTriggerPressed)
            isAnyTriggerPressed = true;
        else
            isAnyTriggerPressed = false;

        /********************** Grip **********************/

        if (gripActionL.action.ReadValue<float>() >= 0.9f)
            isLeftGripPressed = true;
        else
            isLeftGripPressed = false;
        
        if (gripActionR.action.ReadValue<float>() >= 0.9f)
            isRightGripPressed = true;
        else
            isRightGripPressed = false;
        
        if (isLeftGripPressed || isRightGripPressed)
            isAnyGripPressed = true;
        else
            isAnyGripPressed = false;
    }

    public PlayerInputData GetPlayerInput() //return a struct with all local inputs when called
    {
        PlayerInputData playerInputData = new PlayerInputData();

        playerInputData.anyTriggerPressed = (NetworkBool) isAnyTriggerPressed;
        playerInputData.rightTriggerPressed = (NetworkBool) isRightTriggerPressed;
        playerInputData.leftTriggerPressed = (NetworkBool) isLeftTriggerPressed;

        playerInputData.anyGripPressed = (NetworkBool)isAnyGripPressed;
        playerInputData.rightGripPressed = (NetworkBool)isRightGripPressed;
        playerInputData.leftGripPressed = (NetworkBool)isLeftGripPressed;

        return playerInputData;
    }

}
