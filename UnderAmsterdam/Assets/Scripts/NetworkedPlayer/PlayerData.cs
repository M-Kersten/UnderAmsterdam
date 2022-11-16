using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerData : NetworkBehaviour
{
    [Networked] public string company {get; set;}

    [Networked] public int points {get; set;}

    public void ReceiveCompany(string givenCompany) {
            company = givenCompany;
            
    }
}
