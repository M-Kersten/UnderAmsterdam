using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class Pointsmanager : MonoBehaviour
{
    private ConnectionManager coach;
    private int pipeplacepoint = 40;
    private int piperemovepoint = 20;
    private int victorypoints = 400;
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
        PlayerData player = coach._spawnedUsers[CompanyManager.Instance._companies[company]].GetComponent<PlayerData>();
        player.points += piperemovepoint;
        player.myMenu.winPoints(piperemovepoint, false);
    }
    public void RemovePoints(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[CompanyManager.Instance._companies[company]];
        nObject.GetComponent<PlayerData>().points -= pipeplacepoint;
    }

    public void CalculateRoundPoints(string company, bool hasCompleted)
    {
        PlayerData player = coach._spawnedUsers[CompanyManager.Instance._companies[company]].GetComponent<PlayerData>();
        if (hasCompleted)
        {
            player.points += victorypoints;
            player.myMenu.winPoints(victorypoints, true);
        }
        //nObject.GetComponent<PlayerData>().points -= (int)time*10;
    }
}