using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.XR.Host.Rig;

using Fusion.Sockets;
using System;

public class PlayerInput : MonoBehaviour, INetworkRunnerCallbacks
{
    public PlayerInputHandler playerInputHandler;
    public NetworkRig rig;

    private void Start()
    {
        NetworkRunner myNetworkRunner = FindObjectOfType<NetworkRunner>();
        myNetworkRunner.AddCallbacks(this);
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (rig.isActiveAndEnabled && !rig.IsLocalNetworkRig)
        {
            this.enabled = false;
            return;
        }

        if (playerInputHandler == null && NetworkPlayer.LocalPlayer != null)
            playerInputHandler = NetworkPlayer.LocalPlayer.GetComponent<PlayerInputHandler>();

        if (playerInputHandler != null)
            input.Set(playerInputHandler.GetPlayerInput());
/*            {
                Debug.Log("Set" + playerInputHandler.GetPlayerInput().anyTriggerPressed);
            }
            else
                Debug.Log("Not set" + playerInputHandler.GetPlayerInput().anyTriggerPressed);*/

    }

    public void OnEnable()
    {
        NetworkRunner myNetworkRunner = FindObjectOfType<NetworkRunner>();
        myNetworkRunner.AddCallbacks(this);
    }

    public void OnDisable()
    {
        var myNetworkRunner = FindObjectOfType<NetworkRunner>();
        myNetworkRunner.RemoveCallbacks(this);
    }

    #region INetworkRunnerCallbacks (unused)

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }
    #endregion
}