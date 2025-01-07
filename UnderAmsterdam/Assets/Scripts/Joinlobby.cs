using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using TMPro;
using System;
using UnityEngine.Networking;

public class Joinlobby : MonoBehaviour
{
    [SerializeField] private Animator lPlayerAnimator;
    [SerializeField] TextMeshProUGUI textinput;
    private bool canPressButton = true;
    [SerializeField] private float buttonCooldownSeconds = 8;
    
    [ContextMenu("Host join")]
    public async void OnAutoHostJoin()
    {
        StartCoroutine(TestNetworkConnectivity());
        
        ConnectionManager connection = ConnectionManager.Instance;
        if (canPressButton)
        {
            connection.roomName = "testRoom";
            connection.mode = Fusion.GameMode.AutoHostOrClient;

            canPressButton = false;
            StartCoroutine(ButtonCooldown());

            Gamemanager.Instance.localRigid.GetComponent<Animator>().Play("VisionFadeLocal", 0);

            await connection.Connect();

            if (connection.runner.IsServer)
                connection.runner.SetActiveScene(2);
        }
    }
    
    private IEnumerator TestNetworkConnectivity()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://www.google.com");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Network Test Error: " + request.error);
        }
        else
        {
            Debug.Log("Network Test Success: " + request.downloadHandler.text);
        }
    }
    
    private IEnumerator ButtonCooldown()
    {
        yield return new WaitForSeconds(buttonCooldownSeconds);
        canPressButton = true;
    }
}
