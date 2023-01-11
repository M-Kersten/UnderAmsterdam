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
        if (!myRig.IsLocalNetworkRig)
            return;

        if (myWristUI != null && other.gameObject.layer == 8 && !other.CompareTag("Player") && other.name == "Bone")
        {
            Debug.Log("My Rig: " + myRig.IsLocalNetworkRig + " WHO TOUCHED ME: " + other);

            if (myWristUI.activeSelf)
                myWristUI.SetActive(false); 
            else
                myWristUI.SetActive(true);
        }
    }
}
