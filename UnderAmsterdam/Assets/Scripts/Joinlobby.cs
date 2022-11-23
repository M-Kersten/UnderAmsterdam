using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using TMPro;

public class Joinlobby : MonoBehaviour
{
    [SerializeField] ConnectionManager connection;
    [SerializeField] TextMeshProUGUI textinput;
    void Start()
    {
  
    }
    public async void Onjoinlobby()
    {
        connection.roomName = textinput.text;
        connection.mode = Fusion.GameMode.Client;
        await connection.Connect();

    }
    public async void Onhostlobby()
    {
        connection.roomName = textinput.text;
        connection.mode = Fusion.GameMode.Host;
        await connection.Connect();

    }
}
