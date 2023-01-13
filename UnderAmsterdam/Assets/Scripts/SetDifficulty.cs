using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDifficulty : MonoBehaviour
{
    private Gamemanager gameManager;
    [SerializeField] private int timePerRound = 45;
    [SerializeField] private int timeIncreasePerRound = 15;
    [SerializeField] private Valve valve;

    private void Start()
    {
        gameManager = Gamemanager.Instance;
        valve.ValveTurned.AddListener(SetSettings);
    }

    private void SetSettings()
    {
        gameManager.roundTime = timePerRound;
        gameManager.roundTimeIncrease = timeIncreasePerRound;
    }
}
