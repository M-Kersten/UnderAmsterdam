using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host.Rig;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] public GameObject playerCap;
    [SerializeField] private GameObject playerLeftHand, playerRightHand;
    [SerializeField] private GameObject myWatch, topWatch;
    [SerializeField] private int startingPoints = 1000;
    [SerializeField] public WristMenu myMenu;
    [SerializeField] private HandTileInteraction rightHand, leftHand;
    [SerializeField] private Transform leftTransform, rightTransform;
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
        color.SetColour(changed.Behaviour.topWatch, changed.Behaviour.company);
        changed.Behaviour.myMenu.ChangeImage(changed.Behaviour.company);
    }
    private void Start()
    {
        if(Gamemanager.Instance.localData.leftHanded)
        {
            RPC_SwitchHands();
        }

        DontDestroyOnLoad(this.gameObject);
        nRig = GetComponent<NetworkRig>();
        myMenu = GetComponent<NetworkRig>().myMenu;
        points = startingPoints; //Starting amount of points for each player
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SwitchHands()
    {
        //Switching the hands
        rightHand.SwitchHands();
        leftHand.SwitchHands();

        //Moving the watch
        Transform receptionHand = rightHand.isRightHanded ? leftTransform : rightTransform;

        myWatch.transform.parent = receptionHand;
        myWatch.transform.position = receptionHand.position;
        myWatch.transform.rotation = receptionHand.rotation;
    }
}