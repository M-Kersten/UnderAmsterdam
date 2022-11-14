using Fusion.XR.Host;
using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTestMove : MonoBehaviour
{
    [SerializeField] private InputActionProperty joystickLeft;
    [SerializeField] private InputActionProperty joystickRight;
    [SerializeField] private RigPart side;
    [SerializeField] private Transform mainCam;
    [SerializeField] private float speed = 2;

    private CharacterController character;
    private Vector3 direction;
    private Quaternion headQ;

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
        }
        else if (side == RigPart.RightController) {
            side = RigPart.LeftController;
        }
    }
    private void FixedUpdate()
    {
        headQ = Quaternion.Euler(0, mainCam.eulerAngles.y, 0);

        if (side == RigPart.RightController) {
            
            direction = headQ * new Vector3(joystickRight.action.ReadValue<Vector2>().x, 0, joystickRight.action.ReadValue<Vector2>().y);
        }
        else if (side == RigPart.LeftController)
        {
            direction = headQ * new Vector3(joystickLeft.action.ReadValue<Vector2>().x, 0, joystickLeft.action.ReadValue<Vector2>().y);
        }

        character.Move(direction * Time.fixedDeltaTime * speed);
    }
}