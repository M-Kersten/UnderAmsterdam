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
    
    public float roundTime = 45;

    [SerializeField] private Animator lPlayerAnimator;
    [SerializeField] private NetworkRunner runner;
    public bool amIServer;

    [SerializeField] private float roundTimeIncrease = 15;
    [SerializeField] private float roundCountDownTime = 3;
    [SerializeField] private float amountOfRounds = 6;
    [SerializeField] public bool startGame;

    [HideInInspector] public int currentRound;
    [HideInInspector] public Pointsmanager pManager;

    private HostTimerScript timer;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(this);
        }
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
        if(runner.IsConnectedToServer)
        amIServer = runner.IsServer;
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
        Debug.Log("GAMEMANAGER: ON GAME START");
        OnCountDownStart();
    }
    private void OnCountDownStart()
    {
        CountDownStart.Invoke();
        Debug.Log("GAMEMANAGER: ON COUNTDOWN START");

        lPlayerCC.enabled = false;
        StartCoroutine(PreRoundCountDown());
    }
    private void OnCountDownEnd() 
    {
        CountDownEnd.Invoke();
        lPlayerCC.enabled = true;
        Debug.Log("GAMEMANAGER: ON COUNT DOWN END");

        OnRoundStart();
    }
    private void OnRoundStart()
    {
        RoundStart.Invoke();
        Debug.Log("GAMEMANAGER: ON ROUND START");

        timer.SetTimer(roundTime);
        currentRound++;
    }
    private void OnRoundEnd()
    {
        RoundEnd.Invoke();
        Debug.Log("GAMEMANAGER: ON ROUND END");

        roundTime += roundTimeIncrease;
        OnRoundLateEnd();
    }
    private void OnRoundLateEnd()
    {
        RoundLateEnd.Invoke();
        Debug.Log("GAMEMANAGER: ON ROUND LATE END");


        if (currentRound < amountOfRounds)
            OnCountDownStart();
        else
            StartCoroutine(OnGameEnd());
    }
    private IEnumerator OnGameEnd()
    {
        GameEnd.Invoke();
        Debug.Log("GAMEMANAGER: ON GAME END");

        lPlayerAnimator.Play("VisionFadeLocal", 0);
        yield return new WaitForSeconds(lPlayerAnimator.GetCurrentAnimatorClipInfo(0).Length);
        SceneSwitch(3); //EndGame scene
    }
    private IEnumerator PreRoundCountDown()
    {
        yield return new WaitForSeconds(roundCountDownTime);
        OnCountDownEnd();
    }
}
