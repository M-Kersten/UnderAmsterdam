using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host;

public class CityMove : MonoBehaviour
{
    [SerializeField] Dictionary<PlayerRef, bool> playersInGame;
    Vector3 movedown;
    private Animation Anim;
    private Vector3 playerPos;

    void Start()
    {
        playersInGame = new Dictionary<PlayerRef, bool>();
    }

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
            playerPos = Gamemanager.Instance.lPlayerCC.gameObject.transform.position;
            movedown = new Vector3(playerPos.x, -0.5f, playerPos.z);
            StartCoroutine(MovePlayers(playerPos, movedown));
        }
    }
    
    private IEnumerator MovePlayers(Vector3 from, Vector3 to)
    {
        float elapsed = 0;
        float duration = 5f;

        Gamemanager.Instance.lPlayerCC.enabled = false;

        while (elapsed < duration)
        {
            Gamemanager.Instance.lPlayerCC.gameObject.transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Gamemanager.Instance.lPlayerCC.gameObject.transform.position = to;

        Gamemanager.Instance.lPlayerCC.enabled = true;


        if (!Gamemanager.Instance.startGame)
            Gamemanager.Instance.startGame = true;

        yield return null;
    }
}
