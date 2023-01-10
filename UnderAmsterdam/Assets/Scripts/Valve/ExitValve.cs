using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using Fusion.XR.Host;

public class ExitValve : MonoBehaviour
{
    private void ReturnToMenu()
    {
        ConnectionManager.Instance.runner.Disconnect(Gamemanager.Instance.localPlayerData.gameObject.GetComponent<NetworkObject>().InputAuthority);
        SceneManager.LoadScene(0);
        Debug.Log("exit");
    }
}
