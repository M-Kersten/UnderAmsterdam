using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host;
using UnityEngine.Events;

public class CityMove : MonoBehaviour
{
    [SerializeField] Dictionary<PlayerRef, bool> playersInGame;
    Vector3 movedown;
    private Vector3 playerPos;
    private Vector3 moveup;
    [SerializeField] GameObject[] toDisableObjects, toEnableObjects;
    [SerializeField] Material newMaterial;
    [SerializeField] ScoreBoard scoreBoard;
    void Start()
    {
        playersInGame = new Dictionary<PlayerRef, bool>();
        Gamemanager.Instance.GameEnd.AddListener(EndOfGame);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            foreach (PlayerRef player in ConnectionManager.Instance.runner.ActivePlayers)
            {
                if (!playersInGame.ContainsKey(player))
                    playersInGame.Add(player, false);

                Debug.Log("player add: " + player);
            }

            PlayerRef tempplayer = other.GetComponentInParent<NetworkObject>().InputAuthority;

            if (playersInGame.ContainsKey(tempplayer) && !playersInGame[tempplayer])
            {
                playersInGame[tempplayer] = true;
                RPC_EnableCap(other);
                CheckAllPlayers();
            }
        }
    }

    private void CheckAllPlayers()
    {
        int readyPlayers = 0;

        foreach(var player in playersInGame)
        {
            foreach(PlayerRef active in ConnectionManager.Instance.runner.ActivePlayers)
            {
                if(playersInGame.ContainsKey(active))
                {
                    if (playersInGame[active])
                    {
                        readyPlayers++;
                        playersInGame[active] = false;
                        Debug.Log("PLAYER READY: " + active);
                    }
                    Debug.Log("active Player: " + active + " checking player: " + player.Key + " readyPlayers: " + readyPlayers);
                }
                else
                {
                    playersInGame.Remove(player.Key);
                    Debug.Log("Removing: " + player.Key);
                }
            }
        }

        if (playersInGame.Count == readyPlayers && readyPlayers > 0)
        {
            //start animation down
            playerPos = Gamemanager.Instance.lPlayerCC.gameObject.transform.position;
            moveup = playerPos;
            movedown = new Vector3(playerPos.x, -0.5f, playerPos.z);
            StartCoroutine(GameStartProcedure(playerPos, movedown));
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    void RPC_EnableCap(Collider other)
    {
        Debug.Log("enabling cap for: " + other);
        other.transform.root.GetComponent<PlayerData>().playerCap.SetActive(true);
    }

    private void EndOfGame()
    {
        playerPos = Gamemanager.Instance.lPlayerCC.transform.position;
        StartCoroutine(GameOverProcedure(playerPos, moveup));
    }
    private IEnumerator GameStartProcedure(Vector3 from, Vector3 to)
    {
        float elapsed = 0;
        float duration = 5f;

        toDisableObjects[0].SetActive(false);

        //Disable player movement and slide them into the ground
        Gamemanager.Instance.lPlayerCC.enabled = false;
        while (elapsed < duration)
        {
            Gamemanager.Instance.lPlayerCC.gameObject.transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Gamemanager.Instance.lPlayerCC.gameObject.transform.position = to;
        Gamemanager.Instance.lPlayerCC.enabled = true;

        DisableObjectsAfterGameStart();
        Gamemanager.Instance.startGame = true;

        scoreBoard.WarpPlayers();
        yield return null;
    }

    private IEnumerator GameOverProcedure(Vector3 from, Vector3 to)
    {
        float elapsed = 0;
        float duration = 5f;

        EnableObjectsBeforeGameOver();
        toDisableObjects[0].GetComponent<Renderer>().material = newMaterial;

        //Disable player movement and slide them into the ground
        Gamemanager.Instance.lPlayerCC.enabled = false;
        while (elapsed < duration)
        {
            Gamemanager.Instance.lPlayerCC.gameObject.transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Gamemanager.Instance.lPlayerCC.gameObject.transform.position = to;
        Gamemanager.Instance.lPlayerCC.enabled = true;

        toDisableObjects[0].SetActive(true);

        yield return null;
    }

    private void DisableObjectsAfterGameStart()
    {
        for (int i = 0; i < toDisableObjects.Length; i++)
        {
            toDisableObjects[i].SetActive(false);
        }
    }
    private void EnableObjectsBeforeGameOver()
    {
        for (int i = 0; i < toEnableObjects.Length; i++)
        {
            toEnableObjects[i].SetActive(true);
        }
    }
}
