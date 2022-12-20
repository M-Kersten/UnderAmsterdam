using Fusion.XR.Host;
using System.Collections;
using UnityEngine;
using Fusion;

public class StartButtons : MonoBehaviour
{
    private int totalPressed;
    [SerializeField] private int sceneIndex = 1;
    [SerializeField] private Animation Clip;
    [SerializeField] GameObject lobby;
    private NetworkRunner runner;

    void Start() {
        runner = FindObjectOfType<NetworkRunner>();
    }

    public void ButtonStatus(bool pressed)
    {
        if (pressed)
            totalPressed++;
        else
            totalPressed--;

        if (totalPressed == runner.SessionInfo.PlayerCount)
        {
            StartCoroutine(SwitchingScene());
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