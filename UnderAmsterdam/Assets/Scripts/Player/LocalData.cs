using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalData : MonoBehaviour
{
    [SerializeField] public SettingsUI myWristUI;
    [SerializeField] private Transform[] settingsPositions;
    [SerializeField] GameObject leftHand, rightHand;
    public bool leftHanded = false;
    public static LocalData Instance;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SwitchUI()
    {
        if (myWristUI.transform.position == settingsPositions[1].position)
        {
            myWristUI.transform.parent = leftHand.transform;
            myWristUI.transform.position = settingsPositions[0].transform.position;
            myWristUI.transform.rotation = settingsPositions[0].transform.rotation;

        }
        else
        {
            myWristUI.transform.parent = rightHand.transform;
            myWristUI.transform.position = settingsPositions[1].transform.position;
            myWristUI.transform.rotation = settingsPositions[1].transform.rotation;
        }
    }
}
