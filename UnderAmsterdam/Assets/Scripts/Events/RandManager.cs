using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using Fusion;

public class RandManager : NetworkBehaviour
{

    public static RandManager Instance;
    public UnityEvent FlickeringLightsOn, FlickeringLightsOff;

    [Range(0, 5)]
    [SerializeField] private ushort minFlickeringRound;
    private bool haveFlicker = false;

    [SerializeField] private int numberActivatedRoot = 3;
    [SerializeField] private List<GameObject> rootList;

    private int totalPowerPts = 0;
    private int rand;
    
    private CubeInteraction cubeInteraction;
    
     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
     private void RPC_RandRoot(int random)    
     {
         rootList[random].SetActive(true);
     }

    private void Awake()
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
        if (currentRound >= minFlickeringRound && IsPowerConnected < currentRound && !haveFlicker)
        {
            FlickeringLightsOn.Invoke();
            haveFlicker = true;
        } else if (currentRound >= minFlickeringRound && IsPowerConnected >= currentRound)
        {
            FlickeringLightsOff.Invoke();
        }
    }

    public void addPowerPts()
    {
        if (!haveFlicker)
        {
            totalPowerPts++;
        } else
        {
            //Avoid the lights to restart flickering
            totalPowerPts += 1000;
        }
    }

    private void randomActivatedRoots()
    {
        if(!Object.HasStateAuthority)
            return;
        
        for (int i = 0; i < numberActivatedRoot; i++)
        {
            rand = Random.Range(0, rootList.Count - 1);        
            RPC_RandRoot(rand);
        }
    }

}