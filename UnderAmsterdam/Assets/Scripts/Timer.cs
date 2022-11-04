using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timerStart = 600;
    private bool onGoingGame;

    [Tooltip("How much time is given, in seconds")]
    [SerializeField]
    private UnityEvent timerUp;

    private void Start()
    {
        if (timerUp == null)
        {
            timerUp = new UnityEvent();
        }
        timerUp.AddListener(GameState);
    }

    [SerializeField] private TextMeshProUGUI countDownText;

    private void FixedUpdate()
    {
        timerStart = Mathf.Max(0, timerStart - Time.deltaTime);
        TimeSpan timeSpan = TimeSpan.FromSeconds(timerStart);
        countDownText.text = timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
        if (timerStart == 0 && !onGoingGame && timerUp != null) {
            timerUp.Invoke();
        }
    }

    private void GameState()
    {
        onGoingGame = !onGoingGame;
    }
}