using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion.XR.Host.Rig;
using Fusion.XR.Host;


public class MainMenuHands : MonoBehaviour
{
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject realLeftHand;
    [SerializeField] private GameObject realRightHand;
    [SerializeField] private GameObject myMenuWatch;
    [SerializeField] private Transform[] watchPositions;
    [SerializeField] public ParticleSystem attentionLight;

    public static MainMenuHands Instance;

    private HardwareRig myRig;


    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
        myRig = Gamemanager.Instance.localData.GetComponent<HardwareRig>();
        ConnectionManager.Instance.mainMenuDummy = gameObject;

        realLeftHand = myRig.leftHand.gameObject;
        realRightHand = myRig.rightHand.gameObject;
    }

    private void Update() {
        leftHand.transform.position = realLeftHand.transform.position;
        rightHand.transform.position = realRightHand.transform.position;
        
        leftHand.transform.rotation = realLeftHand.transform.rotation;
        rightHand.transform.rotation = realRightHand.transform.rotation;
    }
    public void SwitchWatch()
    {
        if (SceneManager.GetActiveScene().name != "A1Menu") return;
            if (myMenuWatch.transform.position == watchPositions[1].position)
        {
            myMenuWatch.transform.parent = leftHand.transform;
            myMenuWatch.transform.position = watchPositions[0].position;
            myMenuWatch.transform.rotation = watchPositions[0].rotation;
        } else
        {
            myMenuWatch.transform.parent = rightHand.transform;
            myMenuWatch.transform.position = watchPositions[1].position;
            myMenuWatch.transform.rotation = watchPositions[1].rotation;
        }
    }
}
