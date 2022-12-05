using UnityEngine;
using Fusion.XR.Host;
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

    [SerializeField] private Animator lPlayerAnimator;
    [SerializeField] private NetworkRunner runner;

    [SerializeField] private float roundTimeIncrease = 15;
    [SerializeField] private float roundCountDownTime = 3;
    [SerializeField] private float amountOfRounds = 6;
    [SerializeField] private bool startGame;

    [HideInInspector] public Pointsmanager pManager;
    [HideInInspector] public ConnectionManager cManager;
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

    public void SceneSwitch(int index) {
        runner.SetActiveScene(index);
    }

    private void FixedUpdate()
    {
        if (startGame)
        {
            StartCoroutine(OnGameEnd());
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

        if (round <= amountOfRounds)
            OnCountDownStart();
        else
            StartCoroutine(OnGameEnd());
    }
    private IEnumerator OnGameEnd()
    {
        GameEnd.Invoke();
        lPlayerAnimator.Play("VisionFadeLocal", 0);
        yield return new WaitForSeconds(lPlayerAnimator.GetCurrentAnimatorClipInfo(0).Length);
        runner.SetActiveScene(1);
    }
    private IEnumerator PreRoundCountDown()
    {
        yield return new WaitForSeconds(roundCountDownTime);
        OnCountDownEnd();
    }
}
