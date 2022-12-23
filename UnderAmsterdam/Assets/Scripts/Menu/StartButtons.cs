using Fusion.XR.Host;
using System.Collections;
using UnityEngine;
using Fusion;

public class StartButtons : NetworkBehaviour
{
    private int totalPressed;
    [SerializeField] private int sceneIndex = 1;
    [SerializeField] private Animation Clip;
    [SerializeField] GameObject lobby;
    private ConnectionManager cManager;
    private NetworkRunner runner;

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SwitchScene()
    {
        StartCoroutine(SwitchingScene());
    }

    void Start() {
        runner = FindObjectOfType<NetworkRunner>();
        cManager = FindObjectOfType<ConnectionManager>();
    }

    public void ButtonStatus(bool pressed)
    {
        if (pressed)
            totalPressed++;
        else
            totalPressed--;

        if (HasStateAuthority && totalPressed == cManager._spawnedUsers.Count)
        {
            RPC_SwitchScene();
            if (runner.IsServer)
                runner.SessionInfo.IsOpen = false;

        }
    }

    private IEnumerator SwitchingScene()
    {
        Clip.Play();
        lobby.transform.localScale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(Clip.clip.length);
        Gamemanager.Instance.SceneSwitch(sceneIndex);
    }

    public void DevStart()
    {
        Gamemanager.Instance.SceneSwitch(sceneIndex);
    }
}