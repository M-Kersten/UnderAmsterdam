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
    public ExitValveSpawner spawnerRef;
    
    private void Start()
    {
        valve.ValveTurned.AddListener(StartReturnMenu);
        lPlayerAnimator = Gamemanager.Instance.localData.GetComponent<Animator>();
    }

    public void StartReturnMenu()
    {
        if (lPlayerAnimator == null)
            lPlayerAnimator = Gamemanager.Instance.localData.GetComponent<Animator>();
        
        StartCoroutine(ReturnToMenu()); 
    }
    
    public IEnumerator ReturnToMenu()
    {
        spawnerRef.DespawnPipe(gameObject);
        lPlayerAnimator.Play("VisionFadeLocal", 0);
        yield return new WaitForSeconds(lPlayerAnimator.GetCurrentAnimatorClipInfo(0).Length);
        
        Gamemanager.Instance.TeleportToStartPosition();

        if (Gamemanager.Instance.ConnectionManager.runner != null)
            Gamemanager.Instance.ConnectionManager.runner.Shutdown();

        SceneManager.activeSceneChanged += SetupNewMenuScene;
        SceneManager.LoadScene(0);
    }
    
    private void SetupNewMenuScene(Scene scene, Scene scene2)
    {
        Gamemanager.Instance.ConnectionManager.ConnectionSettings.MainMenuDummy = GameObject.Find("MainMenuDummy");
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.layer == 9) // LP is LocalPlayerTag
        {
            spawnerRef.DespawnPipe(gameObject);
        }
    }
}
