using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class Pointsmanager : MonoBehaviour
{
    [SerializeField] private LeaderBoardCanvas leaderBoard;

    private ConnectionManager coach;
    private int pipeplacepoint = -40;
    private int piperemovepoint = 20;
    private int teamworkPoints = 1000;
    private int victorypoints = 4000;

    void Start()
    {
        coach = GetComponent<ConnectionManager>();
    }

    public void AddPoints(string company)
    {
        PlayerData player = coach._spawnedUsers[CompanyManager.Instance._companies[company]].GetComponent<PlayerData>();
        player.points += piperemovepoint;
        player.myMenu.winLosePoints(piperemovepoint);
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
        PlayerData player = coach._spawnedUsers[CompanyManager.Instance._companies[company]].GetComponent<PlayerData>();
        player.points += pipeplacepoint;
        player.myMenu.winLosePoints(pipeplacepoint);
    }

    public void CalculateRoundPoints(string company)
    {
        PlayerData player = coach._spawnedUsers[CompanyManager.Instance._companies[company]].GetComponent<PlayerData>();
        player.points += victorypoints;
        player.myMenu.winLosePoints(victorypoints);
    }
}