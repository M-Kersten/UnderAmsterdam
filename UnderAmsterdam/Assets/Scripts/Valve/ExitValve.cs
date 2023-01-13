using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using Fusion.XR.Host;
using System;

public class ExitValve : MonoBehaviour
{
    [SerializeField] private Valve valve;
    private void Start()
    {
        valve.ValveTurned.AddListener(ReturnToMenu);
    }
    private void ReturnToMenu()
    {
        ConnectionManager.Instance.runner.Disconnect(Gamemanager.Instance.networkData.gameObject.GetComponent<NetworkObject>().InputAuthority);
        SceneManager.LoadScene(0);
        Debug.Log("exit");
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9) // 9 is player dummy, must use local player or other players can disable your pipe
        {
            GetComponent<ExitValveSpawner>().DespawnPipe(gameObject);
        }
    }
}
