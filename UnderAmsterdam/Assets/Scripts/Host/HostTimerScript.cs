using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class HostTimerScript : MonoBehaviour
{
    private float currentTime = 6;
    private bool isGameOngoing;

    [Tooltip("How much time is given, in seconds")]
    [SerializeField] private UnityEvent timerUp;
    
    private void Start()
    {
        if (timerUp == null)
        {
            timerUp = new UnityEvent();
        }
        timerUp.AddListener(SwitchGameState);
    }

    //[SerializeField] private TextMeshProUGUI countDownText;

    private void FixedUpdate()
    {
        currentTime = Mathf.Max(0, currentTime - Time.deltaTime);
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
        
        //Move this to wrist watch stuff
        //countDownText.text = timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
        
        if (currentTime <= 0 && !isGameOngoing && timerUp != null)
        {
            timerUp.Invoke();
        }
    }
    private void SwitchGameState()
    {
        isGameOngoing = !isGameOngoing;
    }
    public void SetTimer(float time)
    {
        if (!isGameOngoing)
        {
            currentTime += time;
            SwitchGameState();
        }
    }
}
