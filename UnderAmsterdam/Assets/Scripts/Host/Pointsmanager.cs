using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;
using Fusion.Addons.ConnectionManagerAddon;

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
    public void AddPoints(int company)
    {
        if (CheckPlayerData(company))
            GetPlayerData(company).points += piperemovepoint;
    }

    public void TeamworkBonus(PlayerRef player)
    {
        if (coach.SpawnedUsers.ContainsKey(player))
        {
            coach.SpawnedUsers[player].GetComponent<PlayerData>().points+= teamworkPoints;
        }
    }

    public void RemovePoints(int company)
    {
        if (CheckPlayerData(company))
            GetPlayerData(company).points += pipeplacepoint;
    }

    public void RemovePointsRoots(int company)
    {
        if (CheckPlayerData(company))
            GetPlayerData(company).points += pipeRootPoints;
    }
    
    public void CalculateRoundPoints(int company)
    {
        if(CheckPlayerData(company))
            GetPlayerData(company).points += victorypoints;
    }

    private bool CheckPlayerData(int company)
    {
        if(companyManager.Companies.ContainsKey(company))
            if (coach.SpawnedUsers.ContainsKey(companyManager.Companies[company]))
                return true;
            else
                return false;
        else
            return false;
    }
    private PlayerData GetPlayerData(int company)
    {
        PlayerData player = coach.SpawnedUsers[companyManager.Companies[company]].GetComponent<PlayerData>();
        return player;
    }
}