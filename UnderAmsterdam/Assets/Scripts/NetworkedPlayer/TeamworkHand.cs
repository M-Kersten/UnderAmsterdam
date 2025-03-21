using Fusion;
using Fusion.XR.Host.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;

public class TeamworkHand : NetworkBehaviour
{
    [SerializeField] private Transform myCap;
    [SerializeField] private NetworkObject myNetworkObj;
    [SerializeField] private PlayerData myData;
    private Transform otherParent;
    [SerializeField] private GameObject teamParticle;
    private GameObject particleObj;

    private void OnTriggerEnter(Collider other)
    {
        if (myNetworkObj.HasStateAuthority && !Gamemanager.Instance.ConnectionManager.runner.SessionInfo.IsOpen)
        {
            if (other.gameObject.layer == 15 && transform.position.y > myCap.position.y)
            {
                otherParent = other.transform.root;
                if (!otherParent.GetComponent<NetworkObject>().HasInputAuthority)
                {
                    if(TeamworkManager.Instance.AddTeamWork(myData.Company, otherParent.GetComponent<PlayerData>().Company))
                        RPC_SendParticle();
                }
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SendParticle()
    {
        particleObj = Instantiate(teamParticle, transform.position, transform.rotation);
        Destroy(particleObj, teamParticle.GetComponent<ParticleSystem>().main.duration + 5f);
    }
}
