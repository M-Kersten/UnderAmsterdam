using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WristUISwitch : MonoBehaviour
{
    [SerializeField] private NetworkObject myObj;
    private SettingsUI wristUI;
    private bool canTouch = true;
    [SerializeField] private int timeInSeconds = 1;


    private void Start()
    {
        wristUI = Gamemanager.Instance.localData.myWristUI;
    }
    public void GetNetworkInfo(SettingsUI myUI)
    {
        wristUI = myUI;
        wristUI.SetRigSliders(myObj);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("WHO: " + other + " TAG: "  + other.tag + " layer: " + other.gameObject.layer);
        if (myObj != null && !myObj.HasInputAuthority)
            return;

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
