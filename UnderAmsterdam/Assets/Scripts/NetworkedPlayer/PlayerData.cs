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

    private ChangeDetector _changes;
    
    [Networked] 
    public int points { get; set; }
    [Networked]
    public int Company { get; set; }

    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            Debug.Log($"change detected: {change}");
            switch (change)
            {
                case nameof(Company):
                    var reader = GetPropertyReader<int>(nameof(Company));
                    var (previous,current) = reader.Read(previousBuffer, currentBuffer);
                    OnCompanyChanged(previous, current);
                    break;
            }
        }
    }

    private void OnCompanyChanged(int oldValue, int newValue)
    {
        UpdatePlayer(newValue);
    }

    private void UpdatePlayer(int newCompany)
    {
        Debug.Log($"updating player to have company: {newCompany}");
        var color = ColourSystem.Instance;
        color.SetColour(playerCap, newCompany);
        color.SetColour(playerLeftHand, newCompany);
        color.SetColour(playerRightHand, newCompany);
        color.SetColour(topWatch, newCompany);
        myMenu.ChangeImage(newCompany);
    }
    
    public void ReceiveCompany(int givenCompany)
    {
        Debug.Log($"assigning company: {givenCompany}");
        Company = givenCompany;
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