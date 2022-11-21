using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private GameObject playerCap;
    [SerializeField] private int startingPoints = 1000;
    [Networked] public string company {get; set;}

    [Networked] public int points {get; set;}

    public void ReceiveCompany(string givenCompany) {
        company = givenCompany;
        ColourSystem.Instance.SetColour(playerCap, company);
    }
    private void Start()
    {
        points = startingPoints; //Starting amount of points for each player
    }
}
