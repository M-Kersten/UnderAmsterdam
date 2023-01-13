using Fusion;
using Fusion.XR.Host.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamworkHand : MonoBehaviour
{
    [SerializeField] private Transform myCap;
    [SerializeField] private NetworkObject myNetworkObj;
    [SerializeField] private PlayerData myData;
    private Transform otherParent;

    private void OnCollisionEnter(Collision collision)
    {
        if (myNetworkObj.HasStateAuthority)
        {
            if (collision.gameObject.layer == 8 && transform.position.y > myCap.position.y)
            {
                otherParent = collision.transform.parent.transform.parent;
                if (!otherParent.GetComponent<NetworkObject>().HasInputAuthority)
                {
                    TeamworkManager.Instance.AddTeamWork(myData.company, otherParent.GetComponent<PlayerData>().company);
                }
            }
        }
    }
}
