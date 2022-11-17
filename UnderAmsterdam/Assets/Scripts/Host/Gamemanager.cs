using UnityEngine;
using Fusion;
using UnityEngine.Events;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance;
    public UnityEvent GameStart, RoundStart, RoundEnd, RoundLateEnd;
    public int round;

    [SerializeField] private float roundTime = 45;
    [SerializeField] private float roundTimeIncrease = 15;
    [SerializeField] private float roundStartCountDown = 3;
    [SerializeField] private bool startGame;
    
    private HostTimerScript timer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        timer = GetComponent<HostTimerScript>();
        timer.timerUp.AddListener(OnRoundEnd);
    }

    private void FixedUpdate()
    {
        if (startGame)
        {
            OnGameStart();
            startGame = false;
        }
    }

    private void OnGameStart()
    {
        GameStart.Invoke();
        timer.SetTimer(roundTime + roundStartCountDown);
        OnRoundStart();
    }
    private void OnRoundStart()
    {
        RoundStart.Invoke();
        round++;
    }
    private void OnRoundEnd()
    {
        RoundEnd.Invoke();
        roundTime += roundTimeIncrease;
        timer.SetTimer(roundTime + roundStartCountDown);
        OnRoundLateEnd();
    }
    private void OnRoundLateEnd()
    {
        RoundLateEnd.Invoke();
        OnRoundStart();
    }
}
