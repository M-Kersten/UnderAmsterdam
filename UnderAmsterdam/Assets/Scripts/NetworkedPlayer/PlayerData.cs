using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host.Rig;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private GameObject playerCap;
    [SerializeField] private int startingPoints = 1000;
    [SerializeField] private WristMenu myMenu;

    [Networked(OnChanged = nameof(UpdatePlayer))]
    public string company { get; set; }

    [Networked] public int points {get; set;}

    public void ReceiveCompany(string givenCompany) {
        company = givenCompany;
    }

    static void UpdatePlayer(Changed<PlayerData> changed)
    {
        ColourSystem.Instance.SetColour(changed.Behaviour.playerCap, changed.Behaviour.company);
        changed.Behaviour.UpdateCompanyImage(changed.Behaviour.company);
    }

    private void UpdateCompanyImage(string company)
    {
        myMenu.ChangeImage(company);
        ColourSystem.Instance.SetColour(myMenu.topWatch, company);
    }
    private void Start()
    {
        myMenu = GetComponent<NetworkRig>().myMenu;
        points = startingPoints; //Starting amount of points for each player
    }
}