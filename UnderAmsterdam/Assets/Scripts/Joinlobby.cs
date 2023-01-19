using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using TMPro;
using System;

public class Joinlobby : MonoBehaviour
{
    [SerializeField] private Animator lPlayerAnimator;
    [SerializeField] ConnectionManager connection;
    [SerializeField] TextMeshProUGUI textinput;
    public async void OnJoinHostLobby()
    {
        lPlayerAnimator.Play("VisionFadeLocal", 0);

        connection.roomName = textinput.text;
        connection.mode = Fusion.GameMode.AutoHostOrClient;

        connection.runner.AddCallbacks(connection);

        await connection.Connect();

        if (connection.Connect().IsCompletedSuccessfully)
        {
            connection.runner.SetActiveScene(2);
        }
    }

    public async void OnJoinLobby()
    {
        lPlayerAnimator.Play("VisionFadeLocal", 0);

        connection.roomName = textinput.text;
        connection.mode = Fusion.GameMode.Client;

        connection.runner.AddCallbacks(connection);

        await connection.Connect();

        if (connection.Connect().IsCompletedSuccessfully)
        {
            connection.runner.SetActiveScene(2);
        }
    }
    public async void onHostLobby()
    {
        lPlayerAnimator.Play("VisionFadeLocal", 0);

        connection.roomName = textinput.text;
        connection.mode = Fusion.GameMode.Host;

        connection.runner.AddCallbacks(connection);

        await connection.Connect();

        if (connection.Connect().IsCompletedSuccessfully)
        {
            connection.runner.SetActiveScene(2);
        }
    }
}
