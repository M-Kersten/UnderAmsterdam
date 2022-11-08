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
            direction = new Vector3(joystickLeft.action.ReadValue<Vector2>().x * Mathf.Cos(transform.rotation.y) + joystickLeft.action.ReadValue<Vector2>().y * Mathf.Sin(transform.rotation.y), 0, joystickLeft.action.ReadValue<Vector2>().y * Mathf.Cos(transform.rotation.y) + joystickLeft.action.ReadValue<Vector2>().x * Mathf.Sin(transform.rotation.y));
        }
        else if (side == RigPart.LeftController)
        {
            direction = new Vector3(joystickLeft.action.ReadValue<Vector2>().x * Mathf.Cos(transform.rotation.y) + joystickLeft.action.ReadValue<Vector2>().y * Mathf.Sin(transform.rotation.y), 0, joystickLeft.action.ReadValue<Vector2>().y * Mathf.Cos(transform.rotation.y) + joystickLeft.action.ReadValue<Vector2>().x * Mathf.Sin(transform.rotation.y));
        }

        character.Move(direction * Time.fixedDeltaTime * speed);
    }
}
