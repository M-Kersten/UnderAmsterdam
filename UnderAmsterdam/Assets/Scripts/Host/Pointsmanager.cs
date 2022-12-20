using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class Pointsmanager : MonoBehaviour
{
    [SerializeField] private LeaderBoardCanvas leaderBoard;

    private ConnectionManager coach;
    private int pipeplacepoint = 500;
    private int piperemovepoint = 200;
    private int victorypoints = 4000;
    private float time;
    private bool roundWinner;

    void Start()
    {
        coach = GetComponent<ConnectionManager>();
    }

    void Update()
    {
        if (roundWinner)
        {
            if (time < victorypoints)
                time += Time.deltaTime;
        }
    }
    public void AddPoints(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[CompanyManager.Instance._companies[company]];
        PlayerData player = nObject.GetComponent<PlayerData>();
        player.points += pipeplacepoint;
        leaderBoard.UpdateLeaderBoard(player);
    }
    public void RemovePoints(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[CompanyManager.Instance._companies[company]];
        PlayerData player = nObject.GetComponent<PlayerData>();
        player.points -= piperemovepoint;
        leaderBoard.UpdateLeaderBoard(player);
    }

    public void CalculateRoundPoints(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[CompanyManager.Instance._companies[company]];

        if (!roundWinner)
        {
            nObject.GetComponent<PlayerData>().points += victorypoints;
            roundWinner = true;
        }
        else
        {
            nObject.GetComponent<PlayerData>().points -= victorypoints - (int)time * 10;
        }
    }
}