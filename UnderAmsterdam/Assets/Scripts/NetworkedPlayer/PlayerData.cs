using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host.Rig;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private GameObject playerCap;
    [SerializeField] private GameObject playerLeftHand, playerRightHand;
    [SerializeField] private int startingPoints = 1500;
    public WristMenu myMenu;
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
        ColourSystem color = ColourSystem.Instance;
        color.SetColour(changed.Behaviour.playerCap, changed.Behaviour.company);
        color.SetColour(changed.Behaviour.playerLeftHand, changed.Behaviour.company);
        color.SetColour(changed.Behaviour.playerRightHand, changed.Behaviour.company);
        changed.Behaviour.UpdateCompanyImage(changed.Behaviour.company);
    }

    private void UpdateCompanyImage(string company)
    {
        if (nRig.IsLocalNetworkRig)
        {
            myMenu.ChangeImage(company);
            ColourSystem.Instance.SetColour(myMenu.topWatch, company);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        nRig = GetComponent<NetworkRig>();
        myMenu = GetComponent<NetworkRig>().myMenu;
        points = startingPoints; //Starting amount of points for each player
    }
}
