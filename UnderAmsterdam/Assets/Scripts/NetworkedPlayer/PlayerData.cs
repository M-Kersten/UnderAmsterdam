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
    [SerializeField] private NetworkObject scoreBoard;
    private ScoreBoard scoreBoardScript;
    private NetworkRig nRig;

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
        if (nRig.IsLocalNetworkRig)
        {
            myMenu.ChangeImage(company);
            ColourSystem.Instance.SetColour(myMenu.topWatch, company);
        }
    }
    void Start()
    {
        Gamemanager.Instance.RoundStart.AddListener(SendPlayerData);
        nRig = GetComponent<NetworkRig>();
        myMenu = GetComponent<NetworkRig>().myMenu;
        points = startingPoints; //Starting amount of points for each player

        scoreBoard = FindObjectOfType<NetworkRunner>().Spawn(scoreBoard, new Vector3(0, -10f, 0));
        scoreBoardScript = scoreBoard.GetComponent<ScoreBoard>();
    }

    private void SendPlayerData()
    {
        scoreBoardScript.SendPlayerData(this);
    }
}
