using Fusion;
using Fusion.XR.Host.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;

public class TeamworkHand : MonoBehaviour
{
    [SerializeField] private Transform myCap;
    [SerializeField] private NetworkObject myNetworkObj;
    [SerializeField] private PlayerData myData;
    private Transform otherParent;
    [SerializeField] private GameObject teamParticle;
    private NetworkObject particleObj;

    private void OnTriggerEnter(Collider other)
    {
        if (myNetworkObj.HasStateAuthority && !ConnectionManager.Instance.runner.SessionInfo.IsOpen)
        {
            Debug.Log("Host: " + myNetworkObj.HasStateAuthority + " Layer: " + other.gameObject.layer + "Hand VS Cap: " + (transform.position.y > myCap.position.y));
            if (other.gameObject.layer == 15 && transform.position.y > myCap.position.y)
            {
                otherParent = other.transform.parent.transform.parent;
                if (!otherParent.GetComponent<NetworkObject>().HasInputAuthority)
                {
                    particleObj = ConnectionManager.Instance.runner.Spawn(teamParticle, transform.position, transform.rotation);
                    StartCoroutine(RemoveParticle());
                    TeamworkManager.Instance.AddTeamWork(myData.company, otherParent.GetComponent<PlayerData>().company);
                }
            }
        }
    }

    IEnumerator RemoveParticle()
    {
        yield return new WaitForSeconds(teamParticle.GetComponent<ParticleSystem>().main.duration);
        ConnectionManager.Instance.runner.Despawn(particleObj);
    }
}
