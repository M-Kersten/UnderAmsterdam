using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using TMPro;
using Fusion;

public class Joinlobby : MonoBehaviour
{
    [SerializeField] ConnectionManager connection;
    [SerializeField] TextMeshProUGUI textinput;
    [SerializeField] NetworkRunner runner;

    public async void OnJoinHostLobby()
    {
        runner.Disconnect(runner.LocalPlayer);
        connection.roomName = textinput.text;
        connection.mode = Fusion.GameMode.AutoHostOrClient;
        await connection.Connect();        
        connection.runner.SetActiveScene(1);
    }
}