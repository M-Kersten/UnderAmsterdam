using UnityEngine;
using Fusion;
using UnityEngine.Events;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance;
    public UnityEvent GameStart, RoundStart, RoundEnd;

    [SerializeField] public float roundTime = 45;
    [SerializeField] private float roundTimeIncrease = 15;
    [SerializeField] private float roundStartCountDown = 3;
    public Pointsmanager pManager;
    
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
        pManager = GetComponent<Pointsmanager>();
        timer = GetComponent<HostTimerScript>();
        timer.timerUp.AddListener(OnRoundEnd);
        OnGameStart();
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
    }
    private void OnRoundEnd()
    {
        RoundEnd.Invoke();
        roundTime += roundTimeIncrease;
        timer.SetTimer(roundTime + roundStartCountDown);
        OnRoundStart();
    }
}
