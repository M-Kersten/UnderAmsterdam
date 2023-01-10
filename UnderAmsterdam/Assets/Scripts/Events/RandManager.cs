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
    [SerializeField] private ushort minFlickeringRound = 0;
    [Range(0,5)]    
    [SerializeField] private ushort minGrowingRound = 0;

    private int totalPowerPts = 0;
    
    private CubeInteraction cubeInteraction;
    
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        
        Gamemanager.Instance.RoundStart.AddListener(checkElecConnection);
        Gamemanager.Instance.RoundEnd.AddListener(randomGrowStep);
    }
    private void checkElecConnection()
    {
        int currentRound = Gamemanager.Instance.currentRound;
        //int IsPowerConnected = WinLoseManager.Instance._inputPipeTracker["power"];
        int IsPowerConnected = totalPowerPts;
        
        // Activate the event only if 
        if (currentRound >= minFlickeringRound && IsPowerConnected < currentRound)
            FlickeringLightsOn.Invoke();
        
        else if (currentRound >= minFlickeringRound && IsPowerConnected >= currentRound )
            FlickeringLightsOff.Invoke();
        
    }    
    private void randomGrowStep()
    {
        if (Gamemanager.Instance.currentRound >= minGrowingRound)
        {
            if (Random.Range(0,1) == 1)
            {
                randomGrowOn.Invoke();
            }
        } 
    }

    public void addPowerPts()
    {
        totalPowerPts++;
    }
    
}
