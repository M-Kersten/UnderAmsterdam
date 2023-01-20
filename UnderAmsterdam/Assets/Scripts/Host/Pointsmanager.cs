using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class Pointsmanager : MonoBehaviour
{
    [SerializeField] ConnectionManager coach;
    private int pipeplacepoint = -40;
    private int pipeRootPoints = -220;
    private int piperemovepoint = 20;
    private int teamworkPoints = 1000;
    private int victorypoints = 4000;

    public void AddPoints(string company)
    {
        PlayerData player = coach._spawnedUsers[CompanyManager.Instance._companies[company]].GetComponent<PlayerData>();
        player.points += piperemovepoint;
    }

    public void TeamworkBonus(string company)
    {
        PlayerData player = coach._spawnedUsers[CompanyManager.Instance._companies[company]].GetComponent<PlayerData>();
        player.points += teamworkPoints;
    }

    public void RemovePoints(string company)
    {
        PlayerData player = coach._spawnedUsers[CompanyManager.Instance._companies[company]].GetComponent<PlayerData>();
        player.points += pipeplacepoint;
    }

    public void RemovePointsRoots(string company)
    {
        PlayerData player = coach._spawnedUsers[CompanyManager.Instance._companies[company]].GetComponent<PlayerData>();
        player.points += pipeRootPoints;
    }
    
    public void CalculateRoundPoints(string company)
    {
        PlayerData player = coach._spawnedUsers[CompanyManager.Instance._companies[company]].GetComponent<PlayerData>();
        player.points += victorypoints;
    }
}