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
        NetworkObject nObject = coach._spawnedUsers[CompanyManager.Instance._companies[company]];
        nObject.GetComponent<PlayerData>().points += piperemovepoint;
    }
    public void RemovePoints(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[CompanyManager.Instance._companies[company]];
        nObject.GetComponent<PlayerData>().points -= pipeplacepoint;
    }

    public void CalculateRoundPoints(string company, bool hasCompleted)
    {
        NetworkObject nObject = coach._spawnedUsers[CompanyManager.Instance._companies[company]];
        if (hasCompleted) nObject.GetComponent<PlayerData>().points += victorypoints;
        //nObject.GetComponent<PlayerData>().points -= (int)time*10;
    }
}