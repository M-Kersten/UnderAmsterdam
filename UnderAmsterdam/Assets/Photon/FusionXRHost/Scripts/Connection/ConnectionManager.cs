using Fusion.Sockets;
using Fusion.XR.Host.Rig;
using System;
using System.Collections;
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
    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Failed,
        Connected
    }

    public class ConnectionManager : MonoBehaviour, INetworkRunnerCallbacks
    {

        public static ConnectionManager Instance;

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

        [SerializeField] private int maxPlayers = 5;

        public PlayerRef localPlayerRef;
        private bool hasPlayerRef = false;

        public NetworkObject localNetworkPlayer;
        private bool hasNetworkPlayer = false;

        [SerializeField] private GameObject connectionManagerPrefab;

        // Dictionary of spawned user prefabs, to destroy them on disconnection
        public Dictionary<PlayerRef, NetworkObject> _spawnedUsers = new Dictionary<PlayerRef, NetworkObject>();

        public ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;

        [SerializeField] private GameObject networkRunnerPrefab;
        [SerializeField] private UpdateConnectionStatus connectionStatusUpdater;
        [SerializeField] private GameObject mainMenuDummy;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private async void Start()
        {
            // Launch the connection at start
            if (connectOnStart) await Connect();
        }
        public async Task Connect()
        {
            //Add network BS to gamemanager
            GameObject newRunner = Instantiate(networkRunnerPrefab);
            runner = newRunner.GetComponent<NetworkRunner>();
            runner.AddCallbacks(this);
            runner.ProvideInput = true;

            runner.AddCallbacks(Gamemanager.Instance.localData.GetComponent<HardwareRig>());

            // Create the scene manager if it does not exist
            if (sceneManager == null) sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

            if (onWillConnect != null) onWillConnect.Invoke();
            // Start or join (depends on gamemode) a session with a specific name
            var args = new StartGameArgs()
            {
                GameMode = mode,
                SessionName = roomName,
                PlayerCount = maxPlayers,

                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = sceneManager,
            };
            await runner.StartGame(args);
        }


        #region INetworkRunnerCallbacks

        public void OnSceneLoadDone(NetworkRunner runner) { 
            Vector3 tPosition;
            Quaternion tRotation;

            if (Gamemanager.Instance.lPlayerCC) {            
                // Turn off CharacterController, so we can teleport the player
                Gamemanager.Instance.lPlayerCC.enabled = false;
                switch (SceneManager.GetActiveScene().name) {
                    case "A1Menu":
                        mainMenuDummy = GameObject.Find("MainMenuDummy");
                        Gamemanager.Instance.FetchLocalPlayerComponents();
                        break;
                    case "A3Game":

                        tPosition = new Vector3(1.02216411f, 4.0f, 1.65285861f);
                        tRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                    
                        Gamemanager.Instance.lPlayerCC.gameObject.transform.position = tPosition;
                        Gamemanager.Instance.lPlayerCC.gameObject.transform.rotation = tRotation;
                        Gamemanager.Instance.lPlayerCC.GetComponent<Animator>().Play("ReverseVisionFadeLocal", 0);
                        break;
                    default:
                    // Do nothing
                    break;
                }
                // Turn CharacterController back on, so player can move
                Gamemanager.Instance.lPlayerCC.enabled = true;
            }
         }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (!hasPlayerRef)
            {
                localPlayerRef = player;
                hasPlayerRef = true;
            }
            // The user's prefab has to be spawned by the host
            if (runner.IsServer)
            {
                // We make sure to give the input authority to the connecting player for their user's object
                NetworkObject networkPlayerObject = runner.Spawn(userPrefab, position: transform.position, rotation: transform.rotation, inputAuthority: player, (runner, obj) => {
                });
                if (!hasNetworkPlayer)
                {
                    localNetworkPlayer = networkPlayerObject;
                    hasNetworkPlayer = true;
                }
                // Keep track of the player avatars so we can remove it when they disconnect
                _spawnedUsers.Add(player, networkPlayerObject);
                //compManage.SendCompany(player, networkPlayerObject);
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
            Gamemanager.Instance.lPlayerCC.GetComponent<Animator>().Play("ReverseVisionFadeLocal", 0);
            mainMenuDummy.SetActive(true);
        }
        public void OnConnectedToServer(NetworkRunner runner)
        {
            Gamemanager.Instance.lPlayerCC.GetComponent<Animator>().Play("VisionFadeLocal", 0);

            Debug.Log(runner.GetPhysicsScene());
            runner.SetActiveScene(2);
        }
        public void OnSceneLoadStart(NetworkRunner runner) { mainMenuDummy.SetActive(false); }
        #endregion

        #region Unused INetworkRunnerCallbacks 
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        private void SetConnectionStatus(ConnectionStatus status) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void LeaveSession() { }
        #endregion
    }
}