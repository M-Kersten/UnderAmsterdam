using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.XR.Host;

public class CityMove : NetworkBehaviour
{
    private List<PlayerRef> readyList= new List<PlayerRef>();
    Vector3 movedown;
    private Vector3 playerPos;
    private Vector3 moveup;
    private bool endOfGame = false;
    [SerializeField] GameObject[] toDisableObjects, toEnableObjects;
    [SerializeField] Material newMaterial;
    [SerializeField] ScoreBoard scoreBoard;
    [SerializeField] private float moveDownY = -0.5f;
    [SerializeField] private GameObject grabText;
    [SerializeField] private GameObject helmetParticles;
    
    void Start()
    {
        Gamemanager.Instance.GameEnd.AddListener(EndOfGame);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            NetworkObject otherObj = other.GetComponentInParent<NetworkObject>();
            PlayerRef temPplayer = otherObj.InputAuthority;
            if(grabText.activeSelf && otherObj.HasInputAuthority)
            {
                helmetParticles.GetComponent<ParticleSystem>().Play();
                grabText.SetActive(false);
            }
            // If I am host
            if (HasStateAuthority && !readyList.Contains(temPplayer))
            {
                readyList.Add(temPplayer);
                CheckAllPlayers();

                // Always enable the cap when a player steps into the ready box first time.
                if (!other.transform.root.GetComponent<PlayerData>().playerCap.activeSelf)
                {
                    RPC_EnableCap(other.transform.root.GetComponent<PlayerData>());   
                }
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
        playerPos = Gamemanager.Instance.localRigid.gameObject.transform.position;
        moveup = playerPos;
        movedown = new Vector3(playerPos.x, moveDownY, playerPos.z);
        StartCoroutine(MovePlayers(playerPos, movedown));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_EnableCap(PlayerData pData)
    {
        pData.playerCap.SetActive(true);
    }

    private void EndOfGame()
    {
        playerPos = Gamemanager.Instance.localRigid.transform.position;
        moveup = new Vector3(playerPos.x, moveup.y, playerPos.z);
        endOfGame = true;
        EnableObjectsBeforeGameOver();
        toDisableObjects[0].GetComponent<Renderer>().material = newMaterial;
        StartCoroutine(MovePlayers(playerPos, moveup));
        toDisableObjects[0].SetActive(true);
    }
    private IEnumerator MovePlayers(Vector3 from, Vector3 to)
    {
        if (!endOfGame)
        {
            toDisableObjects[0].SetActive(false);
            toDisableObjects[1].SetActive(false);
            toDisableObjects[2].SetActive(false);
        }

        float elapsed = 0;
        float duration = 5f;

        while (elapsed < duration)
        {
            Gamemanager.Instance.localRigid.gameObject.transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!endOfGame)
        {
            Gamemanager.Instance.OnGameStart();
            DisableObjectsAfterGameStart();
        }

        Gamemanager.Instance.localRigid.gameObject.transform.position = to;
        if (endOfGame)
        {
            Gamemanager.Instance.localRigid.GetComponent<Animator>().Play("VisionFadeLocal", 0);
            yield return new WaitForSeconds(1f);
            scoreBoard.WarpPlayers();
            Gamemanager.Instance.localRigid.GetComponent<Animator>().Play("ReverseVisionFadeLocal", 0);
        }
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
