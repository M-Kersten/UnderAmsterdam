using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;


public class PlayerInputHandler : MonoBehaviour
{
    public InputActionProperty joystickActionL;
    public InputActionProperty joystickActionR;

    public InputActionProperty triggerActionL;
    public InputActionProperty triggerActionR;

    public InputActionProperty gripActionL;
    public InputActionProperty gripActionR;

    public InputActionProperty ExitButton;

    private RigPart side;

    public Vector2 leftjoystickPosition;
    public Vector2 rightjoystickPosition;

    [SerializeField]
    public bool isAnyTriggerPressed;
    [SerializeField]
    public bool isRightTriggerPressed;
    [SerializeField]
    public bool isLeftTriggerPressed;

    public bool isAnyGripPressed;
    public bool isRightGripPressed;
    public bool isLeftGripPressed;

    /*----- Not linked to network -----*/
    public bool isMenuPressed;
    /*---------------------------------*/


    private void Awake()
    {
        side = RigPart.LeftController;
        triggerActionL.EnableWithDefaultXRBindings(side: side, new List<string> { "trigger" });
        gripActionL.EnableWithDefaultXRBindings(side: side, new List<string> { "grip" });
        joystickActionL.EnableWithDefaultXRBindings(side: side, new List<string> { "joystick" });
        ExitButton.EnableWithDefaultXRBindings(side: side, new List<string> { "secondaryButton" });

        side = RigPart.RightController;
        triggerActionR.EnableWithDefaultXRBindings(side: side, new List<string> { "trigger" });
        gripActionR.EnableWithDefaultXRBindings(side: side, new List<string> { "grip" });
        joystickActionR.EnableWithDefaultXRBindings(side: side, new List<string> { "joystick" });
    }

    private void Update()
    {

        /********************* Trigger *********************/

        isLeftTriggerPressed = triggerActionL.action.ReadValue<float>() >= 0.9f;
        isRightTriggerPressed = triggerActionR.action.ReadValue<float>() >= 0.9f;

        leftjoystickPosition = joystickActionL.action.ReadValue<Vector2>();
        rightjoystickPosition = joystickActionR.action.ReadValue<Vector2>();

        isAnyTriggerPressed = isLeftTriggerPressed || isRightTriggerPressed;

        /********************** Grip **********************/

        isLeftGripPressed = gripActionL.action.ReadValue<float>() >= 0.9f;
        isRightGripPressed = gripActionR.action.ReadValue<float>() >= 0.9f;

        isAnyGripPressed = isLeftGripPressed || isRightGripPressed;

        /********************** ExitButton **********************/
    
        isMenuPressed = ExitButton.action.ReadValue<float>() >= 0.9f;
    }

    public RigInput GetPlayerInput(RigInput playerInputData) //return a struct with all local inputs when called
    {
        playerInputData.anyTriggerPressed = isAnyTriggerPressed;
        playerInputData.rightTriggerPressed = isRightTriggerPressed;
        playerInputData.leftTriggerPressed = isLeftTriggerPressed;

        playerInputData.anyGripPressed = isAnyGripPressed;
        playerInputData.rightGripPressed = isRightGripPressed;
        playerInputData.leftGripPressed = isLeftGripPressed;

        return playerInputData;
    }

}