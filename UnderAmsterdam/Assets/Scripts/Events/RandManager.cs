using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandManager : MonoBehaviour
{
    [SerializeField] private UInt16 minFlickeringRound;
    private bool 
    void Start()
    {
        Gamemanager.Instance.RoundStart.AddListener(checkElecConnection);
    }
    private void checkElecConnection()
    {
        if (Gamemanager.Instance.round >= minFlickeringRound && 
            WinLoseManager.Instance._inputPipeTracker["power"] <= (minFlickeringRound - 1))
        {
            
        }
    }
}
