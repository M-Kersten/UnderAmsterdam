using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristUISwitch : MonoBehaviour
{
    private GameObject myWristUI;

    private void OnTriggerEnter(Collider other)
    {
        if (myWristUI == null)
        {
            myWristUI = Gamemanager.Instance.lPlayerCC.transform.GetChild(1).GetChild(0).gameObject;
        }
        if(other.gameObject.layer == 8)
        {
            if(myWristUI.activeSelf)
                myWristUI.SetActive(false); 
            else
                myWristUI.SetActive(true);
        }
    }
}
