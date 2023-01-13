using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host.Rig;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private GameObject playerCap;
    [SerializeField] private GameObject playerLeftHand, playerRightHand;
    [SerializeField] private GameObject myWatch;
    [SerializeField] private int startingPoints = 1000;
    [SerializeField] public WristMenu myMenu;
    [SerializeField] private HandTileInteraction rightHand, leftHand;
    [SerializeField] private Transform leftTransform, rightTransform;
    private Transform localLeftHand, localRightHand;
    private NetworkRig nRig;
    [SerializeField] private WristUISwitch watchUI;

    [Networked(OnChanged = nameof(UpdatePlayer))]
    public string company { get; set; }

    [Networked] public int points { get; set; }

    public void ReceiveCompany(string givenCompany)
    {
        company = givenCompany;
    }

    static void UpdatePlayer(Changed<PlayerData> changed)
    {
        ColourSystem color = ColourSystem.Instance;
        color.SetColour(changed.Behaviour.playerCap, changed.Behaviour.company);
        color.SetColour(changed.Behaviour.playerLeftHand, changed.Behaviour.company);
        color.SetColour(changed.Behaviour.playerRightHand, changed.Behaviour.company);
        changed.Behaviour.myMenu.ChangeImage(changed.Behaviour.company);
    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        nRig = GetComponent<NetworkRig>();
        myMenu = GetComponent<NetworkRig>().myMenu;
        points = startingPoints; //Starting amount of points for each player

        watchUI.GetNetworkInfo(nRig, Gamemanager.Instance.localData.myWristUI);

        localLeftHand = Gamemanager.Instance.lPlayerCC.transform.GetChild(1);
        localLeftHand = localLeftHand.GetChild(localLeftHand.childCount - 1);
        localRightHand = Gamemanager.Instance.lPlayerCC.transform.GetChild(2);
        localRightHand = localRightHand.GetChild(localRightHand.childCount - 1);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SwitchHands()
    {
        //Switching the hands
        rightHand.isRightHanded = !rightHand.isRightHanded;
        leftHand.isRightHanded = !leftHand.isRightHanded;

        //Moving the watch
        Transform receptionHand = rightHand.isRightHanded ? leftTransform : rightTransform;

        myWatch.transform.SetParent(receptionHand);
        myWatch.transform.position = receptionHand.position;
        myWatch.transform.rotation = receptionHand.rotation;
    }
}