using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class Pointsmanager : MonoBehaviour
{
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
        nObject.GetComponent<PlayerData>().points += pipeplacepoint;
    }
    public void RemovePoints(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[CompanyManager.Instance._companies[company]];
        nObject.GetComponent<PlayerData>().points -= piperemovepoint;
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