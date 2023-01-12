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
    private int teamworkPoints = 1000;

    void Start()
    {
        coach = GetComponent<ConnectionManager>();
    }

    public void AddPoints(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[CompanyManager.Instance._companies[company]];
        PlayerData player = nObject.GetComponent<PlayerData>();
        player.points += pipeplacepoint;
        leaderBoard.UpdateLeaderBoard(player);
    }
    public void TeamworkBonus(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[CompanyManager.Instance._companies[company]];
        PlayerData player = nObject.GetComponent<PlayerData>();
        player.points += teamworkPoints;
        leaderBoard.UpdateLeaderBoard(player);
    }
    public void RemovePoints(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[CompanyManager.Instance._companies[company]];
        PlayerData player = nObject.GetComponent<PlayerData>();
        player.points -= piperemovepoint;
        leaderBoard.UpdateLeaderBoard(player);
    }
}