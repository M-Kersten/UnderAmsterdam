using Fusion.XR.Host;
using System.Collections;
using UnityEngine;

public class StartButtons : MonoBehaviour
{
    private int totalPressed;
    [SerializeField] private int sceneIndex = 1;
    [SerializeField] private Animation Clip;
    [SerializeField] private Animation Clip2;

    public void ButtonStatus(bool pressed)
    {
        if (pressed)
            totalPressed++;
        else
            totalPressed--;

        if (totalPressed == ConnectionManager.Instance._spawnedUsers.Count)
        {
            StartCoroutine(SwitchingScene());
        }
    }

    private IEnumerator SwitchingScene()
    {
        Clip.Play();
        Clip2.Play();
        yield return new WaitForSeconds(Clip.clip.length);
        Gamemanager.Instance.SceneSwitch(sceneIndex);
    }

    public void DevStart()
    {
        Gamemanager.Instance.SceneSwitch(sceneIndex);
    }
}