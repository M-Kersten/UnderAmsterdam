using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristUISwitch : MonoBehaviour
{
    private NetworkRig myRig;
    private SettingsUI wristUI;


    private void Start()
    {
        wristUI = Gamemanager.Instance.localData.myWristUI;
    }
    public void GetNetworkInfo(NetworkRig givenRig, SettingsUI myUI)
    {
        wristUI = myUI;
        myRig = givenRig;
        Debug.Log("MyRig: " + myRig  + " UI: " + wristUI);
        wristUI.SetRigSliders(myRig);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("WHO: " + other + " TAG: "  + other.tag + " layer: " + other.gameObject.layer);
        if (myRig != null && !myRig.IsLocalNetworkRig)
            return;

        if (wristUI != null && other.gameObject.layer == 8 && other.CompareTag("UI"))
        {
            if (MainMenuHands.Instance != null && MainMenuHands.Instance.attentionLight.gameObject.activeSelf)
                MainMenuHands.Instance.attentionLight.gameObject.SetActive(false);

            if (wristUI.gameObject.activeSelf)
                wristUI.gameObject.SetActive(false);
            else
                wristUI.gameObject.SetActive(true);
        }
    }
}
