using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class GridDisplay : NetworkBehaviour, INetworkRunnerCallbacks
{
    //public NetworkObject block;
    public NetworkObject cube;
    public Transform parent;

    public Vector3 StartPos;
    public uint width = 8;
    public uint height = 4;
    public uint depth = 8;
    //spublic GameObject[,,] GridA; //Array with all the cubes
    public NetworkObject[,,] GridA; //Array with all the cubes

    private protected uint nbPipes;
    public NetworkRunner runner;


    public override void Spawned()
    {
        GridA = new NetworkObject[width, height, depth];
        //GridA = new NetworkObject[width, height, depth];
        for (uint x = 0; x < width; ++x)
        {
            for (uint y = 0; y < height; ++y)
            {
                for (uint z = 0; z < depth; ++z)
                {
                    GridA[x, y, z] = runner.Spawn(cube, new Vector3(StartPos.x + x * 0.5f, StartPos.y + y * 0.5f, 0.5f * z + StartPos.z), Quaternion.identity,runner.LocalPlayer);
                    //GridA[x, y, z] = Instantiate(cube, new Vector3(StartPos.x + x * 0.5f, StartPos.y + y * 0.5f, 0.5f * z + StartPos.z), Quaternion.identity, parent);
                    GridA[x, y, z].transform.name = "Cube: " + x + ' ' + y + ' ' + z;
                    ++nbPipes;
                }
            }
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        GridA = new NetworkObject[width, height, depth];
        //GridA = new NetworkObject[width, height, depth];
        for (uint x = 0; x < width; ++x)
        {
            for (uint y = 0; y < height; ++y)
            {
                for (uint z = 0; z < depth; ++z)
                {
                    GridA[x, y, z] = runner.Spawn(cube, new Vector3(StartPos.x + x * 0.5f, StartPos.y + y * 0.5f, 0.5f * z + StartPos.z), Quaternion.identity, runner.LocalPlayer);
                    //GridA[x, y, z] = Instantiate(cube, new Vector3(StartPos.x + x * 0.5f, StartPos.y + y * 0.5f, 0.5f * z + StartPos.z), Quaternion.identity, parent);
                    GridA[x, y, z].transform.name = "Cube: " + x + ' ' + y + ' ' + z;
                    ++nbPipes;
                }
            }
        }
        throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
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

    public void OnDisconnectedFromServer(NetworkRunner runner)
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

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
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
