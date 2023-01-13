using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristUISwitch : MonoBehaviour
{
    private GameObject myWristUI;
    [SerializeField] public NetworkRig myRig;

    private void Start()
    {
        if (myWristUI == null)
        {
            myWristUI = Gamemanager.Instance.lPlayerCC.transform.GetChild(1).GetChild(0).gameObject;
            myWristUI.GetComponent<SettingsUI>().SetRigSliders(myRig);
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("WHO: " + other + " TAG: "  + other.tag + " layer: " + other.gameObject.layer);
        if (myRig != null && !myRig.IsLocalNetworkRig)
            return;

        if (myWristUI != null && other.gameObject.layer == 8 && other.CompareTag("UI"))
        {
            if (MainMenuHands.Instance != null && MainMenuHands.Instance.attentionLight.gameObject.activeSelf)
                MainMenuHands.Instance.attentionLight.gameObject.SetActive(false);

            if (myWristUI.activeSelf)
                myWristUI.SetActive(false); 
            else
                myWristUI.SetActive(true);
        }
    }
}
