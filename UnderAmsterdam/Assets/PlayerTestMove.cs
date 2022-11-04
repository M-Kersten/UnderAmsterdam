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

    float speed = 2;
    void Start()
    {
        character = GetComponent<CharacterController>();

        side = RigPart.LeftController;
        joystickLeft.EnableWithDefaultXRBindings(side: side, new List<string> { "joystick" });

        side = RigPart.RightController;
        joystickRight.EnableWithDefaultXRBindings(side: side, new List<string> { "joystick" });
    }
    private void FixedUpdate()
    {
        Vector3 direction;
        if (side == RigPart.RightController) {
            direction = new Vector3(joystickLeft.action.ReadValue<Vector2>().x, 0, joystickLeft.action.ReadValue<Vector2>().y);
        }
        else if (side == RigPart.LeftController)
        {
            direction = new Vector3(joystickLeft.action.ReadValue<Vector2>().x, 0, joystickLeft.action.ReadValue<Vector2>().y);
        }
        else
        {
            direction = new Vector3(0,0,0);
        }

        character.Move(direction * Time.fixedDeltaTime * speed);
    }
}
