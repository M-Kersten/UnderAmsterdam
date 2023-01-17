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
    [SerializeField] Material[] newMaterials;
    [SerializeField] ScoreBoard scoreBoard;
    private bool gameEnded;

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
            }

            PlayerRef tempplayer = other.GetComponentInParent<NetworkObject>().InputAuthority;

            if (playersInGame.ContainsKey(tempplayer) && !playersInGame[tempplayer])
            {
                playersInGame[tempplayer] = true;
                CheckAllPlayers();
            }
        }
    }
    private void CheckAllPlayers()
    {
        int readyPlayers = 0;
        foreach (var player in playersInGame)
        {
            if (player.Value)
            {
                readyPlayers++;
            }
        }
        if (playersInGame.Count == readyPlayers && readyPlayers > 0)
        {
            //start animation down
            Debug.Log("play animation");
            playerPos = Gamemanager.Instance.lPlayerCC.gameObject.transform.position;
            moveup = playerPos;
            movedown = new Vector3(playerPos.x, -0.5f, playerPos.z);
            StartCoroutine(MovePlayers(playerPos, movedown));
        }
    }
    private void EndOfGame()
    {
        gameEnded = true;
        playerPos = Gamemanager.Instance.lPlayerCC.gameObject.transform.position;
        StartCoroutine(MovePlayers(playerPos, moveup));
    }

    private IEnumerator MovePlayers(Vector3 from, Vector3 to)
    {
        float elapsed = 0;
        float duration = 5f;

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

        //Swap objects depending on gamestate
        if (gameEnded)
        {
            EnableObjectsAfterGameOver();
            GetComponent<Renderer>().material = newMaterials[1];
            //toEnableObjects[1].GetComponent<Renderer>().material = newMaterials[0];
            //toDisableObjects[0].GetComponent<Renderer>().material = newMaterials[1];
            scoreBoard.warpPlayers();
        }
        else
        {
            DisableObjectsAfterGameOver();
        }

        if (!gameEnded && !Gamemanager.Instance.startGame)
            Gamemanager.Instance.startGame = true;

        yield return null;
    }
    private void DisableObjectsAfterGameOver()
    {
        for (int i = 0; i < toDisableObjects.Length; i++)
        {
            toDisableObjects[i].SetActive(false);
        }
    }
    private void EnableObjectsAfterGameOver()
    {
        for (int i = 0; i < toEnableObjects.Length; i++)
        {
            toEnableObjects[i].SetActive(true);
        }
    }
}
