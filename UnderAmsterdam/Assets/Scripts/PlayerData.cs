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

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_ReceiveCompany([RpcTarget] PlayerRef targetPlayer, string givenCompany, RpcInfo info = default)
    {
        Debug.Log("Received Company " + givenCompany + " From " + info.Source);
            company = givenCompany;
    }
}
