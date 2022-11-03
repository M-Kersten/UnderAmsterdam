using System;
using UnityEngine;
using UnityEngine.Events;

public class HostTimerScript : MonoBehaviour
{
    [Tooltip("How much time is given, in seconds")]
    public UnityEvent timerUp;

    [SerializeField] private float currentTime = -1;
    private bool isGameOngoing;
    
    private void Start()
    {
        if (timerUp == null)
        {
            timerUp = new UnityEvent();
        }
        timerUp.AddListener(SwitchGameState);
    }

    private void FixedUpdate()
    {
        currentTime = Mathf.Max(0, currentTime - Time.deltaTime);
        
        if (currentTime == 0 && isGameOngoing)
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
