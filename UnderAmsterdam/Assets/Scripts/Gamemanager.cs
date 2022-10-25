using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.Events;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance;

    public UnityEvent GameStart, RoundStart, RoundEnd;
    private float gameTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        gameTime += Time.deltaTime;
    }

    private void OnGameStart()
    {
        GameStart.Invoke();
    }
    private void OnRoundStart()
    {
        RoundStart.Invoke();

    }
    private void OnRoundEnd()
    {
        RoundEnd.Invoke();
    }
}
