using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class TestRPC : NetworkBehaviour
{
    [SerializeField]
    string text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Object.InputAuthority && Input.GetKeyDown(KeyCode.R))
        {
            RPC_SendMessage(text);
        }
    }

    private Text _messages;

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendMessage(string message, RpcInfo info = default)
    {
        if (info.IsInvokeLocal)
           Debug.Log($"You said: {message}\n");
        else
            Debug.Log($"Some other player said: {message}\n");
    }
}
