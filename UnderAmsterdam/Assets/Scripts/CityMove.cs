using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host;

public class CityMove : MonoBehaviour
{
    [SerializeField] List<PlayerRef> playerlist;
    private Animation Anim;
    private void OnTriggerEnter(Collider other)
        {
        if (other.gameObject.layer == 8)
        {
            for (int i = 0; i < playerlist.Count; i++)
            {
                PlayerRef tempplayer = other.GetComponentInParent<NetworkObject>().InputAuthority; 
                if (!playerlist[i] == tempplayer)
                {
                    playerlist.Add(tempplayer);
                }
            }
        }
        }
    private void CheckAllPlayers()
    {
        if (playerlist.Count == ConnectionManager.Instance._spawnedUsers.Count) {
            //start animation
            Anim.Play("CityMove");
        }
    }   
}
