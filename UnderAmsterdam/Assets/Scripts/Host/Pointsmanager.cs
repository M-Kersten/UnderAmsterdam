using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class Pointsmanager : MonoBehaviour
{
    [SerializeField] ConnectionManager coach;
    private CompanyManager companyManager;
    private int pipeplacepoint = -40;
    private int pipeRootPoints = -220;
    private int piperemovepoint = 20;
    private int teamworkPoints = 1000;
    private int victorypoints = 4000;
    private void Start()
    {
        companyManager = CompanyManager.Instance;
    }
    public void AddPoints(string company)
    {
        if (CheckPlayerData(company))
            GetPlayerData(company).points += piperemovepoint;
    }

    public void TeamworkBonus(PlayerRef player)
    {
        if (coach._spawnedUsers.ContainsKey(player))
        {
            coach._spawnedUsers[player].GetComponent<PlayerData>().points+= teamworkPoints;
        }
    }

    public void RemovePoints(string company)
    {
        if (CheckPlayerData(company))
            GetPlayerData(company).points += pipeplacepoint;
    }

    public void RemovePointsRoots(string company)
    {
        if (CheckPlayerData(company))
            GetPlayerData(company).points += pipeRootPoints;
    }
    
    public void CalculateRoundPoints(string company)
    {
        if(CheckPlayerData(company))
            GetPlayerData(company).points += victorypoints;
    }

    private bool CheckPlayerData(string company)
    {
        if(companyManager._companies.ContainsKey(company))
            if (coach._spawnedUsers.ContainsKey(companyManager._companies[company]))
                return true;
            else
                return false;
        else
            return false;
    }
    private PlayerData GetPlayerData(string company)
    {
        PlayerData player = coach._spawnedUsers[companyManager._companies[company]].GetComponent<PlayerData>();
        return player;
    }
}