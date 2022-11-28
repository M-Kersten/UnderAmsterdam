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

    private RigPart side;

    public Vector2 leftjoystickPosition;
    public Vector2 rightjoystickPosition;

    [SerializeField]
    private bool isAnyTriggerPressed;
    [SerializeField]
    private bool isRightTriggerPressed;
    [SerializeField]
    private bool isLeftTriggerPressed;

    [SerializeField]
    public bool isAnyGripPressed;

    public bool isRightGripPressed;

    public bool isLeftGripPressed;


    private void Awake()
    {
        side = RigPart.LeftController;
        triggerActionL.EnableWithDefaultXRBindings(side: side, new List<string> { "trigger" });
        gripActionL.EnableWithDefaultXRBindings(side: side, new List<string> { "grip" });
        joystickActionL.EnableWithDefaultXRBindings(side: side, new List<string> { "joystick" });

        side = RigPart.RightController;
        triggerActionR.EnableWithDefaultXRBindings(side: side, new List<string> { "trigger" });
        gripActionR.EnableWithDefaultXRBindings(side: side, new List<string> { "grip" });
        joystickActionR.EnableWithDefaultXRBindings(side: side, new List<string> { "joystick" });
    }

    private void Update()
    {
        /********************* Trigger *********************/

        isLeftTriggerPressed = (triggerActionL.action.ReadValue<float>() >= 0.9f);
        isRightTriggerPressed = (triggerActionR.action.ReadValue<float>() >= 0.9f);

        leftjoystickPosition = joystickActionL.action.ReadValue<Vector2>();
        rightjoystickPosition = joystickActionR.action.ReadValue<Vector2>();

        isAnyTriggerPressed = (isLeftTriggerPressed || isRightTriggerPressed);

        /********************** Grip **********************/

        isLeftGripPressed = (gripActionL.action.ReadValue<float>() >= 0.9f);
        isRightGripPressed = (gripActionR.action.ReadValue<float>() >= 0.9f);

        isAnyGripPressed = (isLeftGripPressed || isRightGripPressed);

    }

    public RigInput GetPlayerInput(RigInput playerInputData) //return a struct with all local inputs when called
    {
        playerInputData.anyTriggerPressed = (NetworkBool)isAnyTriggerPressed;
        playerInputData.rightTriggerPressed = (NetworkBool)isRightTriggerPressed;
        playerInputData.leftTriggerPressed = (NetworkBool)isLeftTriggerPressed;

        playerInputData.anyGripPressed = (NetworkBool)isAnyGripPressed;
        playerInputData.rightGripPressed = (NetworkBool)isRightGripPressed;
        playerInputData.leftGripPressed = (NetworkBool)isLeftGripPressed;

        return playerInputData;
    }

}
