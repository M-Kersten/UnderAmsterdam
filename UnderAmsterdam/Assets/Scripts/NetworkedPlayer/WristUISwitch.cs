using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WristUISwitch : NetworkBehaviour
{
    [SerializeField] private bool input;
    private SettingsUI wristUI;
    private bool canTouch = true;
    [SerializeField] private int timeInSeconds = 1;


    private void Start()
    {
        wristUI = Gamemanager.Instance.localData.myWristUI;
        Debug.Log("My WristUI: " + wristUI + " GameObject: " + Gamemanager.Instance.localData.myWristUI.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.GetComponent<NetworkObject>() != null) {
            if (!other.transform.root.GetComponent<NetworkObject>().HasInputAuthority)
                return;
        }

        if (wristUI != null && other.gameObject.layer == 8 && other.CompareTag("UI") && canTouch)
        {
            StartCoroutine(TouchTimer(timeInSeconds));
            if (MainMenuHands.Instance != null && MainMenuHands.Instance.attentionLight.gameObject.activeSelf)
                MainMenuHands.Instance.attentionLight.gameObject.SetActive(false);

            if (wristUI.gameObject.activeSelf)
                wristUI.gameObject.SetActive(false);
            else
                wristUI.gameObject.SetActive(true);
        }
    }
    private IEnumerator TouchTimer(int seconds)
    {
        canTouch = false;
        yield return new WaitForSeconds(seconds);
        canTouch = true;
    }
}
