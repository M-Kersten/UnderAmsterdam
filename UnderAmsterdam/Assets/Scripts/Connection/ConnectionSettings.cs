using UnityEngine;
using System.Collections;
using Fusion;
using UnityEngine.Serialization;

[System.Serializable]
public class ConnectionSettings
{
    public string MainScene = nameof(MainScene);
    public  int MaxPlayers = 5;
    public PlayerRef LocalPlayerRef;
    public bool HasPlayerRef = false;
    public GameObject MainMenuDummy;
}