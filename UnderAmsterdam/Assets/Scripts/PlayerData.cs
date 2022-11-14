using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerData : NetworkBehaviour
{
    [Networked] public string company {get; set;}

    public int points;

    public void ReceiveCompany(string givenCompany) {
            company = givenCompany;
    }
}
