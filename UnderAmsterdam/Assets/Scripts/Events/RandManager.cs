using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class RandManager : MonoBehaviour
{
    public static RandManager Instance;
    public UnityEvent FlickeringLightsOn, FlickeringLightsOff, randomGrowOn;

    [Range(0,5)]    
    [SerializeField] private ushort minFlickeringRound;
    [Range(0,5)]    
    [SerializeField] private ushort minGrowingRound;
    
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        
        Gamemanager.Instance.RoundStart.AddListener(checkElecConnection);
        Gamemanager.Instance.RoundStart.AddListener(randomGrowStep);
    }
    private void checkElecConnection()
    {
        // Activate the event only if 
        if (Gamemanager.Instance.round >= minFlickeringRound && 
            WinLoseManager.Instance._inputPipeTracker["power"] < Gamemanager.Instance.round)
        {
            FlickeringLightsOn.Invoke();
        } 
        else if (Gamemanager.Instance.round >= minFlickeringRound &&
                   WinLoseManager.Instance._inputPipeTracker["power"] >= Gamemanager.Instance.round )
        {
            FlickeringLightsOff.Invoke();
        }
    }    
    private void randomGrowStep()
    {
        if (Gamemanager.Instance.round >= minGrowingRound)
        {
            if (Random.Range(0,1) == 1)
            {
                randomGrowOn.Invoke();
            }
        } 
    }
}
