using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuHands : MonoBehaviour
{
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject realLeftHand;
    [SerializeField] private GameObject realRightHand;

    private void Update() {
        leftHand.transform.position = realLeftHand.transform.position;
        rightHand.transform.position = realRightHand.transform.position;
        
        leftHand.transform.rotation = realLeftHand.transform.rotation;
        rightHand.transform.rotation = realRightHand.transform.rotation;
    }
}
