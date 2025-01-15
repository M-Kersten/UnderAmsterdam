using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;

    public Dictionary<string, int> InputPipeTracker;
    
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        SetAllPipeTrackers(0);

        Gamemanager.Instance.RoundStart.AddListener(FlushList);
        Gamemanager.Instance.RoundLateEnd.AddListener(CheckWhoWin);
    }
    
    public void AddInputTracker(string company)
    {
        InputPipeTracker[company] += 1;
    }

    private void FlushList()
    {
        SetAllPipeTrackers(0);
    }

    void SetAllPipeTrackers(int value)
    {
        InputPipeTracker = new Dictionary<string, int>();
        foreach (CompanyType type in Enum.GetValues(typeof(CompanyType)))
            InputPipeTracker[type.ToString()] = value;
    }

    private void CheckWhoWin()
    {
        foreach(var company in InputPipeTracker)
        {
            if(company.Value >= Gamemanager.Instance.currentRound)
            {
                //Add points to this company
                // Gamemanager.Instance.pManager.AddPoints(company.Key);
            }
        }
    }
}
