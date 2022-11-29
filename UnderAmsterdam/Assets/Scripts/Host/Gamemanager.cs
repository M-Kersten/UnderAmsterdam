using UnityEngine;
using Fusion;
using UnityEngine.Events;
using System.Collections;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance;

    public UnityEvent GameStart, RoundStart, RoundEnd, RoundLateEnd, GameEnd, CountDownStart, CountDownEnd;
    public PlayerData localPlayerData;
    public CharacterController lPlayerCC;

    public int round;
    public float roundTime = 45;

    [SerializeField] private float roundTimeIncrease = 15;
    [SerializeField] private float roundCountDownTime = 3;
    [SerializeField] private float amountOfRounds = 6;
    [SerializeField] private bool startGame;

    [HideInInspector] public Pointsmanager pManager;
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
        OnCountDownStart();
    }
    private void OnCountDownStart()
    {
        CountDownStart.Invoke();
        lPlayerCC.enabled = false;
        StartCoroutine(PreRoundCountDown());
    }
    private void OnCountDownEnd() 
    {
        CountDownEnd.Invoke();
        lPlayerCC.enabled = true;
        OnRoundStart();
    }
    private void OnRoundStart()
    {
        RoundStart.Invoke();
        timer.SetTimer(roundTime);
        round++;
    }
    private void OnRoundEnd()
    {
        RoundEnd.Invoke();
        roundTime += roundTimeIncrease;
        OnRoundLateEnd();
    }
    private void OnRoundLateEnd()
    {
        RoundLateEnd.Invoke();

        if (round < amountOfRounds)
            OnCountDownStart();
        else
            OnGameEnd();
    }
    private void OnGameEnd()
    {
        GameEnd.Invoke();
    }
    private IEnumerator PreRoundCountDown()
    {
        yield return new WaitForSeconds(roundCountDownTime);
        OnCountDownEnd();
    }
}
