using Fusion.Sockets;
using Fusion.XR.Host.Rig;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Fusion.XR.Host
{
    /**
     * 
     * Handles:
     * - connexion launch
     * - user representation spawn on connection (on the host)
     * - user despawn by the host on associated player disconnection
     * 
     **/
    
    public class ConnectionManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public ConnectionSettings ConnectionSettings;
        
        [Header("Room configuration")]
        public GameMode mode = GameMode.AutoHostOrClient;
        public string roomName = "SampleFusionVR";
        public bool connectOnStart = false;

        [Header("Fusion settings")]
        [Tooltip("Fusion runner. Automatically created if not set")]
        public NetworkRunner runner;
        public INetworkSceneManager sceneManager;

        [Header("Local user spawner")]
        public NetworkObject userPrefab;

        [Header("Event")]
        public UnityEvent onWillConnect = new UnityEvent();

        // Dictionary of spawned user prefabs, to destroy them on disconnection
        private Dictionary<PlayerRef, NetworkObject> _spawnedUsers = new Dictionary<PlayerRef, NetworkObject>();

        public Dictionary<PlayerRef, NetworkObject> SpawnedUsers => _spawnedUsers;
        
        private async void Start()
        {
            // Launch the connection at start
            if (connectOnStart) await Connect();
        }
        
        public async Task Connect()
        {
            SetupCustomConnection();

            // Create the scene manager if it does not exist
            if (sceneManager == null) sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

            if (onWillConnect != null) onWillConnect.Invoke();
            // Start or join (depends on gamemode) a session with a specific name
            var args = new StartGameArgs()
            {
                GameMode = mode,
                SessionName = roomName,
                PlayerCount = ConnectionSettings.MaxPlayers,

                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = sceneManager
            };
            await runner.StartGame(args);
        }

        private void SetupCustomConnection()
        {
            var newRunner = Instantiate(ConnectionSettings.NetworkRunnerPrefab);
            runner = newRunner.GetComponent<NetworkRunner>();
            runner.AddCallbacks(this);
            runner.ProvideInput = true;

            runner.AddCallbacks(Gamemanager.Instance.localData.GetComponent<HardwareRig>());
        }

        #region INetworkRunnerCallbacks

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Gamemanager.Instance.TeleportToStartPosition();
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!ConnectionSettings.HasPlayerRef)
            {
                ConnectionSettings.localPlayerRef = player;
                ConnectionSettings.HasPlayerRef = true;
            }
            // The user's prefab has to be spawned by the host
            if (runner.IsServer)
            {
                // We make sure to give the input authority to the connecting player for their user's object
                NetworkObject networkPlayerObject = runner.Spawn(userPrefab, position: transform.position, rotation: transform.rotation, inputAuthority: player, (runner, obj) => {
                });
                if (!ConnectionSettings.HasNetworkPlayer)
                {
                    ConnectionSettings.localNetworkPlayer = networkPlayerObject;
                    ConnectionSettings.HasNetworkPlayer = true;
                }
                // Keep track of the player avatars so we can remove it when they disconnect
                _spawnedUsers.Add(player, networkPlayerObject);
            }
        }

        // Despawn the user object upon disconnection
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // Find and remove the players avatar (only the host would have stored the spawned game object)
            if (_spawnedUsers.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedUsers.Remove(player);
            }
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Gamemanager.Instance.ShutdownPlayer(runner);
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            runner.SetActiveScene(2);
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            if(ConnectionSettings.MainMenuDummy != null) 
                ConnectionSettings.MainMenuDummy.SetActive(false);
        }
        #endregion

        #region Unused INetworkRunnerCallbacks 
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) {}
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        #endregion
    }
}