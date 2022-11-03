using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerJoystickMovement : MonoBehaviour
{
    PlayerInputHandler localPlayerInput;

    private float speed = 2;
    private void Start()
    {
        localPlayerInput = GetComponent<PlayerInputHandler>();
    }
    private void FixedUpdate()
    {
        transform.position += new Vector3(transform.position.x + localPlayerInput.leftjoystickPosition.x, transform.position.y, transform.position.z + localPlayerInput.leftjoystickPosition.y) * speed;
    }
}
