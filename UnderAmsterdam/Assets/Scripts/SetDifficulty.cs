using Fusion;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SetDifficulty : NetworkBehaviour
{
    [SerializeField] private int timePerRound = 45;
    [SerializeField] private int timeIncreasePerRound = 15;
    [SerializeField] private Valve valve;
    [SerializeField] private Transform selectionO;
    [SerializeField] private Transform finalTransform;
    [SerializeField] private Valve[] OtherValve;

    private float _animationDuration = 3;

    private void Start()
    {
        valve.ValveTurned.AddListener(RPCValve);
        foreach (var oValve in OtherValve)
            oValve.ValveTurned.AddListener(RPCChangeValve);
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPCValve()
    {
        Gamemanager.Instance.roundTime = timePerRound;
        Gamemanager.Instance.roundTimeIncrease = timeIncreasePerRound;
        
        selectionO.gameObject.SetActive(true);

        DOTween.Sequence()
            .Append(selectionO.DOMove(finalTransform.position, _animationDuration))
            .Append(selectionO.DORotateQuaternion(finalTransform.rotation, _animationDuration));
        
        Debug.Log("Settings set" + timePerRound + " " + timeIncreasePerRound);
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPCChangeValve()
    {
        selectionO.localPosition = new Vector3(0,-0.5f,0);
        selectionO.gameObject.SetActive(false);
    }
}
