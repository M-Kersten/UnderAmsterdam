using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host.Rig;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private GameObject playerCap;
    [SerializeField] private GameObject playerHands;
    [SerializeField] private int startingPoints = 1000;
    [SerializeField] private WristMenu myMenu;
    private NetworkRig nRig;

    [Networked(OnChanged = nameof(UpdatePlayer))]
    public string company { get; set; }

    [Networked] public int points { get; set; }

    public void ReceiveCompany(string givenCompany)
    {
        company = givenCompany;
    }

    static void UpdatePlayer(Changed<PlayerData> changed)
    {
        ColourSystem.Instance.SetColour(changed.Behaviour.playerCap, changed.Behaviour.company);
        ColourSystem.Instance.SetColour(changed.Behaviour.playerHands, changed.Behaviour.company);
    }
}
