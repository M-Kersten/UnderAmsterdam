using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Time;

public class SetDifficulty : NetworkBehaviour
{
    private Gamemanager gameManager;
    [SerializeField] private int timePerRound = 45;
    [SerializeField] private int timeIncreasePerRound = 15;
    [SerializeField] private Valve valve;
    [SerializeField] private Transform selectionO;
    [SerializeField] private Transform finalTransform;
    [SerializeField] private Valve[] OtherValve;

    private void Start()
    {
        gameManager = Gamemanager.Instance;
        valve.ValveTurned.AddListener(RPCValve);
        foreach (Valve oValve in OtherValve)
        {
            oValve.ValveTurned.AddListener(RPCChangeValve);
        }
        
    }

    private IEnumerator MoveSelection()
    {
        float currentTime = 0;
        
        while (currentTime < 3f)
        {
            float t = currentTime / 3f;
            selectionO.position = Vector3.Lerp(selectionO.position, finalTransform.position, t);
            selectionO.rotation = Quaternion.Lerp(selectionO.rotation, finalTransform.rotation, t);
            
            currentTime += Time.deltaTime;
            yield return null;
        }
        yield return true;
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPCValve()
    {
        gameManager.roundTime = timePerRound;
        gameManager.roundTimeIncrease = timeIncreasePerRound;
        
        selectionO.gameObject.SetActive(true);
        StartCoroutine(MoveSelection());
        Debug.Log("Settings set" + timePerRound + " " + timeIncreasePerRound);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPCChangeValve()
    {
        selectionO.localPosition = new Vector3(0,-0.5f,0);
        selectionO.gameObject.SetActive(false);
    }
}
