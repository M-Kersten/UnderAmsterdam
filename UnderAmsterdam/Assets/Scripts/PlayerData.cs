using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerData : NetworkBehaviour
{
    PlayerRef player;
    int playerId;

    public string company { get; set; }

    string prevCompany;
    int points;

    void Start()
    {
        playerId = player.PlayerId;
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
