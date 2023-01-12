using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class RandManager : MonoBehaviour
{
    public static RandManager Instance;
    public UnityEvent FlickeringLightsOn, FlickeringLightsOff;

    [Range(0,5)]    
    [SerializeField] private ushort minFlickeringRound = 0;
    [Range(0,5)]    
    [SerializeField] private ushort minGrowingRound = 0;
    
    [SerializeField] private int numberActivatedRoot = 3;
    
    [SerializeField] private List<GameObject> rootList;

    private int totalPowerPts = 0;
    
    private CubeInteraction cubeInteraction;
    
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        
        Gamemanager.Instance.RoundStart.AddListener(checkElecConnection);
        Gamemanager.Instance.GameStart.AddListener(randomActivatedRoots);
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
    
    private void randomActivatedRoots()
    {
        for (int i = 0; i < numberActivatedRoot; i++)
        {
            int rand = Random.Range(0, rootList.Count - 1);
            rootList[rand].SetActive(true);
        }
    }

    public void addPowerPts()
    {
        totalPowerPts++;
    }
    
}
