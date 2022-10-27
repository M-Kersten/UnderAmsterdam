using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour
{
    public static NetworkPlayer LocalPlayer { get; set; }
    public override void Spawned()
    {
        base.Spawned();
        if (Object.HasInputAuthority)
        {
            LocalPlayer = this;
        }
    }
}
