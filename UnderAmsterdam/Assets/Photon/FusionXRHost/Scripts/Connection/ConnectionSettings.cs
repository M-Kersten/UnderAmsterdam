using UnityEngine;
using System.Collections;
using Fusion;

[System.Serializable]
public class ConnectionSettings
{
    [SerializeField] private int maxPlayers = 5;
    public PlayerRef localPlayerRef;
    public NetworkObject localNetworkPlayer;
    public bool HasPlayerRef = false;
    public bool HasNetworkPlayer = false;
    
    [HideInInspector]
    public ConnectionStatus ConnectionStatus = ConnectionStatus.Disconnected;

    public GameObject NetworkRunnerPrefab;
    public UpdateConnectionStatus ConnectionStatusUpdater;
    public GameObject MainMenuDummy;
    
    public GameObject ConnectionManagerPrefab;

    public int MaxPlayers
    {
        get { return maxPlayers; }
        set { maxPlayers = value; }
    }

    public PlayerRef LocalPlayerRef
    {
        get { return localPlayerRef; }
        set 
        {
            localPlayerRef = value; 
            HasPlayerRef = localPlayerRef != null; 
        }
    }

    public NetworkObject LocalNetworkPlayer
    {
        get { return localNetworkPlayer; }
        set 
        { 
            localNetworkPlayer = value; 
            HasNetworkPlayer = localNetworkPlayer != null; 
        }
    }
}