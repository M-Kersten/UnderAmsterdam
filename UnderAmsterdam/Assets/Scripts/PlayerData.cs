using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerData : MonoBehaviour
{
    PlayerRef player;
    int playerId;

    [Networked] public string company { get; set; }

    string prevCompany;
    int points;

    void Start()
    {
        playerId = player.PlayerId;
    }

    public void ReceivePlayerCompany(string givenCompany)
    {
        RPC_SetCompany(givenCompany);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    void RPC_SetCompany(string givenCompany)
    {
        this.company = givenCompany;
    }
}
