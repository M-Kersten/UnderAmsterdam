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

    public void ReceiveCompany(string givenCompany)
    {
        this.company = givenCompany;
        //RPC_SetCompany(givenCompany);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SetCompany(string givenCompany)
    {
        this.company = givenCompany;
    }
}
