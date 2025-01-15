using System;
using Fusion.XR.Host;
using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTestMove : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private InputActionProperty joystickLeft;
    [SerializeField] private InputActionProperty joystickRight;
    [SerializeField] private RigPart side;
    [SerializeField] private Transform mainCam;

    [Range(50f, 150f)]
    [SerializeField] private float speed = 70f;

    private Rigidbody mainRigid;

    private CharacterController character;
    private Vector3 direction;
    private Quaternion headQ;
    private bool _inputReceived;

    void Start()
    {
        // https://prnt.sc/FCTsqNJwEysf
        DontDestroyOnLoad(this.gameObject);

        _inputReceived = false;
        
        character = GetComponent<CharacterController>();
        mainRigid = GetComponent<Rigidbody>();
        
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
        
        if (mainRigid != null && Gamemanager.Instance.ConnectionManager.runner != null && _inputReceived)
            mainRigid.velocity = (direction * Gamemanager.Instance.ConnectionManager.runner.DeltaTime * speed);
        else if (mainRigid != null)
            mainRigid.velocity = (direction * Time.fixedDeltaTime * speed);
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        _inputReceived = true;
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
}