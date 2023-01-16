using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using TMPro;

public class Joinlobby : MonoBehaviour
{
    [SerializeField] ConnectionManager connection;
    [SerializeField] TextMeshProUGUI textinput;
    public async void OnJoinHostLobby()
    {
        connection.roomName = textinput.text;
        connection.mode = Fusion.GameMode.AutoHostOrClient;
        await connection.Connect();

        connection.runner.SetActiveScene(2);
    }
}
