using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host;
using UnityEngine.Events;
using Photon.Realtime;

public class CityMove : NetworkBehaviour
{
    private List<PlayerRef> readyList= new List<PlayerRef>();
    Vector3 movedown;
    private Vector3 playerPos;
    private Vector3 moveup;
    [SerializeField] GameObject[] toDisableObjects, toEnableObjects;
    [SerializeField] Material newMaterial;
    [SerializeField] ScoreBoard scoreBoard;
    void Start()
    {
        Gamemanager.Instance.GameEnd.AddListener(EndOfGame);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            PlayerRef tempplayer = other.GetComponentInParent<NetworkObject>().InputAuthority;
            // If I am host
            if (HasStateAuthority && !readyList.Contains(tempplayer))
            {
                readyList.Add(tempplayer);
                CheckAllPlayers();

                // Always enable the cap when a player steps into the ready box first time.
                if (!other.transform.root.GetComponent<PlayerData>().playerCap.activeSelf)
                    RPC_EnableCap(other.transform.root.GetComponent<PlayerData>());
            }
        }
    }

    private void CheckAllPlayers()
    {
        int readyPlayers = 0;
        // Check if players in ready list are still in game
        for(int i = 0; i < readyList.Count; i++)
        {
            if (ConnectionManager.Instance._spawnedUsers.ContainsKey(readyList[i]))
                readyPlayers++;
            else
                readyList.Remove(readyList[i]);
        }

        if (ConnectionManager.Instance._spawnedUsers.Count == readyPlayers && readyPlayers > 0)
        {
            RPC_StartGame();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_StartGame()
    {
        //start animation down
        playerPos = Gamemanager.Instance.lPlayerCC.gameObject.transform.position;
        moveup = playerPos;
        movedown = new Vector3(playerPos.x, -0.5f, playerPos.z);
        GameStartProcedure(playerPos, movedown);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_EnableCap(PlayerData pData)
    {
        pData.playerCap.SetActive(true);
    }

    private void EndOfGame()
    {
        playerPos = Gamemanager.Instance.lPlayerCC.transform.position;
        GameOverProcedure(playerPos, moveup);
    }
    private void GameStartProcedure(Vector3 from, Vector3 to)
    {
        toDisableObjects[0].SetActive(false);

        StartCoroutine(MovePlayers(from, to));

        DisableObjectsAfterGameStart();
        Gamemanager.Instance.startGame = true;

        scoreBoard.WarpPlayers();
    }

    private void GameOverProcedure(Vector3 from, Vector3 to)
    {
        EnableObjectsBeforeGameOver();
        toDisableObjects[0].GetComponent<Renderer>().material = newMaterial;

        StartCoroutine(MovePlayers(from, to));

        toDisableObjects[0].SetActive(true);
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
    }

    private void DisableObjectsAfterGameStart()
    {
        this.GetComponent<BoxCollider>().enabled = false;
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
