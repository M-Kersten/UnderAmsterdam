using Fusion.XR.Host;
using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTestMove : MonoBehaviour
{
    public InputActionProperty joystickLeft;
    public InputActionProperty joystickRight;

    public RigPart side;

    public CharacterController character;

    public Transform headset;

    public float speed = 2;
    void Start()
    {
        character = GetComponent<CharacterController>();
        

        side = RigPart.RightController;
        joystickRight.EnableWithDefaultXRBindings(side: side, new List<string> { "joystick" });

        side = RigPart.LeftController;
        joystickLeft.EnableWithDefaultXRBindings(side: side, new List<string> { "joystick" });
    }

    public void SwitchMovementControles()
    {
        if (side == RigPart.LeftController) {
            side = RigPart.RightController;
            joystickRight.EnableWithDefaultXRBindings(side: side, new List<string> { "joystick" });
        }
        else if (side == RigPart.RightController) {
            side = RigPart.LeftController;
            joystickLeft.EnableWithDefaultXRBindings(side: side, new List<string> { "joystick" });
        }
        
    }
    private void FixedUpdate()
    {
        Vector3 direction = new Vector3();

        if (side == RigPart.RightController) {
            Quaternion headQ = Quaternion.Euler(0, headset.eulerAngles.y, 0);
            direction = headQ * new Vector3(joystickRight.action.ReadValue<Vector2>().x, 0, joystickRight.action.ReadValue<Vector2>().y);
        }
        else if (side == RigPart.LeftController)
        {
            Quaternion headQ = Quaternion.Euler(0, headset.eulerAngles.y, 0);
            direction = headQ * new Vector3(joystickLeft.action.ReadValue<Vector2>().x, 0, joystickLeft.action.ReadValue<Vector2>().y);
        }

        character.Move(direction * Time.fixedDeltaTime * speed);
    }
}
