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

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            GetComponent<TestRPC>().RPC_SendMessage("Server test");
        }
    }

    public void ReceiveCompany(PlayerRef targetPlayer, string givenCompany)
    {
        RPC_SendMessage(targetPlayer, givenCompany);
    }

    [Rpc]
    private void RPC_SendMessage([RpcTarget] PlayerRef targetPlayer, string givenCompany, RpcInfo info = default)
    {
            company = givenCompany;
    }
}
