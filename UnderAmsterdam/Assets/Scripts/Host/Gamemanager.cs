using UnityEngine;
using Fusion.XR.Host;
using Fusion;
using UnityEngine.Events;
using System.Collections;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance;

    public UnityEvent GameStart, RoundStart, RoundEnd, RoundLateEnd, GameEnd, CountDownStart, CountDownEnd;

    public PlayerData networkData;

    [SerializeField] private GameObject localPlayer; 
    public LocalData localData;
    public PlayerInputHandler playerInputHandler;
    public CharacterController lPlayerCC;
    public Transform mainCam;

    public float roundTimeIncrease = 15;
    public float roundTime = 45;

    [SerializeField] private NetworkRunner runner;

    [SerializeField] private float roundCountDownTime = 3;
    
    public int amountOfRounds = 5;
    public bool startGame;

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

        FetchLocalPlayerComponents();
    }
    private void Start()
    {
        pManager = GetComponent<Pointsmanager>();
        timer = GetComponent<HostTimerScript>();
        timer.timerUp.AddListener(OnRoundEnd);
    }
    public void FetchLocalPlayerComponents()
    {
        localPlayer = GameObject.Find("LocalPlayer");
        Debug.Log(localPlayer);
        localData = localPlayer.GetComponent<LocalData>();
        playerInputHandler = localPlayer.GetComponent<PlayerInputHandler>();
        lPlayerCC = localPlayer.GetComponent<CharacterController>();
        mainCam = localPlayer.transform.GetChild(0).GetChild(0);
    }

    public void SceneSwitch(int index) {
        runner.SetActiveScene(index);
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
        ConnectionManager.Instance.runner.SessionInfo.IsOpen = false;
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
            OnGameEnd();
    }
    private IEnumerator PreRoundCountDown()
    {
        yield return new WaitForSeconds(roundCountDownTime);
        OnCountDownEnd();
    }
    private void OnGameEnd()
    {
        GameEnd.Invoke();
        Debug.Log("GAMEMANAGER: ON GAME END");
    }
}
