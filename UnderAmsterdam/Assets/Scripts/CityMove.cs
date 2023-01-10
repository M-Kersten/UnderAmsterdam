using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host;

public class CityMove : MonoBehaviour
{
    [SerializeField] Dictionary<PlayerRef, bool> playersInGame;
    private Animation Anim;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            foreach (PlayerRef player in ConnectionManager.Instance.runner.ActivePlayers)
            {
                Debug.Log("player" + player);
                if (!playersInGame.ContainsKey(player))
                    playersInGame.Add(player, false);
            }

            PlayerRef tempplayer = other.GetComponentInParent<NetworkObject>().InputAuthority;

            if(playersInGame.ContainsKey(tempplayer) && !playersInGame[tempplayer])
            {
                playersInGame[tempplayer] = true;
                CheckAllPlayers();
            }
        }
    }
    private void CheckAllPlayers()
    {
        int readyPlayers = 0;
        foreach(var player in playersInGame)
        {
            if (player.Value)
            {
                readyPlayers++;
            }
        }
        if (playersInGame.Count == readyPlayers && readyPlayers > 0) {
            //start animation
            Debug.Log ("play animation"); 
            Anim.Play("CityUp");
        }
    }   
}
