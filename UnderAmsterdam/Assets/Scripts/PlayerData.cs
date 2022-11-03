using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerData : NetworkBehaviour
{
    PlayerRef player;
    int playerId;

    [Networked] public string company {get; set;}

    string prevCompany;
    int points;

    // Store some values that others can grab from here
    void Start()
    {
        playerId = player.PlayerId;
    }

    //// RPC, activated by Host, targeted at which player needs to be activated and to update at all clients
    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //public void RPC_ReceiveCompany([RpcTarget] PlayerRef targetPlayer, string givenCompany, RpcInfo info = default)
    //{
    //    company = givenCompany;
    //}

    public void ReceiveCompany(string givenCompany) {
        company = givenCompany;
    }
}
