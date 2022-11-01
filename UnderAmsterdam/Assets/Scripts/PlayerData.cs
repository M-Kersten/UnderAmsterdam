using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerData : NetworkBehaviour
{
    PlayerRef player;
    int playerId;

    public string company;

    string prevCompany;
    int points;

    void Start()
    {
        playerId = player.PlayerId;
    }

    [Networked]
    string networkedtext { get; set; }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]

    public void RPC_SendMessage()

    {

        networkedtext = new System.Random().Next(1, 11).ToString();

        Debug.Log("RPC executed");

    }

    public void ReceivePlayerCompany(string givenCompany)
    {
        this.company = givenCompany;
        //RPC_SetCompany(givenCompany);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    void RPC_SetCompany(string givenCompany)
    {
        this.company = givenCompany;
    }
}
