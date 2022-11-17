using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    private float timerStart = 0;
    private bool onGoingGame;

    private void Start()
    {
        Gamemanager.Instance.RoundStart.AddListener(GameState);
        Gamemanager.Instance.RoundEnd.AddListener(GameState);
    }

    [SerializeField] private TextMeshProUGUI countDownText;

    private void FixedUpdate()
    {
        if (onGoingGame)
        {
            timerStart = Mathf.Max(0, timerStart - Time.deltaTime);
            TimeSpan timeSpan = TimeSpan.FromSeconds(timerStart);
            countDownText.text = timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
        } else if (timerStart != Gamemanager.Instance.roundTime) 
        {
            timerStart = Gamemanager.Instance.roundTime;
        }
    }

    private void GameState()
    {
        onGoingGame = !onGoingGame;
    }
}