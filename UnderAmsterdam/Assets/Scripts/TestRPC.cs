using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class TestRPC : NetworkBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Text _messages;

    [Rpc]
    public void RpcTest([RpcTarget] PlayerRef targetPlayer, RpcInfo info = default)
    {
        if (info.IsInvokeLocal)
           Debug.Log("Sent");
        else
            Debug.Log("Received: " + targetPlayer.PlayerId); ;
    }
}
