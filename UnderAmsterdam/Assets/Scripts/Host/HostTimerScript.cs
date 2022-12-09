using System;
using UnityEngine;
using UnityEngine.Events;

public class HostTimerScript : MonoBehaviour
{
    [Tooltip("How much time is given, in seconds")]
    public UnityEvent timerUp;

    private float currentTime = -1;
    public bool isATimerOngoing;
    
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
        
        if (currentTime == 0 && isATimerOngoing)
        {
            timerUp.Invoke();
        }
    }
    private void SwitchGameState()
    {
        isATimerOngoing = !isATimerOngoing;
    }
    //When set timer is called the gamestate is set to on going untill time up is invoked.
    public void SetTimer(float time)
    {
        if (!isATimerOngoing)
        {
            currentTime += time;
            SwitchGameState();
        }
    }
}
