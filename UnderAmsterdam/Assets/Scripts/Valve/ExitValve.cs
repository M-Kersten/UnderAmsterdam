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
    [SerializeField] Animator lPlayerAnimator;
    private void Start()
    {
        valve.ValveTurned.AddListener(StartReturnMenu);
        lPlayerAnimator = Gamemanager.Instance.localData.GetComponent<Animator>();
    }
    private void StartReturnMenu() { StartCoroutine(ReturnToMenu()); }
    public IEnumerator ReturnToMenu()
    {
        lPlayerAnimator.Play("VisionFadeLocal", 0);
        yield return new WaitForSeconds(lPlayerAnimator.GetCurrentAnimatorClipInfo(0).Length);

        if (ConnectionManager.Instance.runner != null)
            ConnectionManager.Instance.runner.Shutdown();

        SceneManager.LoadScene(0);
        Destroy(lPlayerAnimator.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9) // 9 is player dummy, must use local player or other players can disable your pipe
        {
            GetComponent<ExitValveSpawner>().DespawnPipe(gameObject);
        }
    }
}
