using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalData : MonoBehaviour
{
    [SerializeField] public SettingsUI myWristUI;
    [SerializeField] private Transform[] settingsPositions;
    [SerializeField] GameObject realLeftHand, realRightHand;

    public void SwitchWatch()
    {
        if (myWristUI.transform.position == settingsPositions[1].position)
        {
            myWristUI.transform.parent = realLeftHand.transform;
            myWristUI.transform.position = settingsPositions[0].transform.position;
            myWristUI.transform.rotation = settingsPositions[0].transform.rotation;

        }
        else
        {
            myWristUI.transform.parent = realRightHand.transform;
            myWristUI.transform.position = settingsPositions[1].transform.position;
            myWristUI.transform.rotation = settingsPositions[1].transform.rotation;
        }
    }
}
