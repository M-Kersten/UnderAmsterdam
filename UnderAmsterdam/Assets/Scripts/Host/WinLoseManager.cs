using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;

    public Dictionary<string, int> _inputPipeTracker;
    private void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        _inputPipeTracker = new Dictionary<string, int> {
            {"water", 0},
            {"gas", 0},
            {"data", 0},
            {"sewage", 0},
            {"power", 0}
        };

        Gamemanager.Instance.RoundStart.AddListener(FlushList);
        Gamemanager.Instance.RoundLateEnd.AddListener(CheckWhoWin);
    }
    public void AddInputTracker(string company)
    {
        _inputPipeTracker[company] += 1;
    }

    private void FlushList()
    {
        _inputPipeTracker = new Dictionary<string, int> {
            {"water", 0},
            {"gas", 0},
            {"data", 0},
            {"sewage", 0},
            {"power", 0}
        };
    }

    private void CheckWhoWin()
    {
        foreach(var company in _inputPipeTracker)
        {
            if(company.Value >= Gamemanager.Instance.round)
            {
                //Add points to this company
                // Gamemanager.Instance.pManager.AddPoints(company.Key);
            }
        }
    }
}
