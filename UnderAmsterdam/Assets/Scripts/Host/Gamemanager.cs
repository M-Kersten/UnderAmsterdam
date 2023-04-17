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

    public GameObject localPlayer;
    public LocalData localData;
    public PlayerInputHandler playerInputHandler;
    public Rigidbody localRigid;
    public Transform mainCam;

    public float roundTimeIncrease = 10;
    public float roundTime = 45;

    [SerializeField] private NetworkRunner runner;

    [SerializeField] private float roundCountDownTime = 3;

    public int amountOfRounds = 5;

    public int currentRound;
    [HideInInspector] public Pointsmanager pManager;

    private HostTimerScript timer;

    private float defaultRoundTimeIncrease = 10, defaultRoundTime = 45;

    public bool gameOngoing;

    private void Awake()
    {
        if (Instance == null)
        {
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
        localData = localPlayer.GetComponent<LocalData>();
        playerInputHandler = localPlayer.GetComponent<PlayerInputHandler>();
        localRigid = localPlayer.GetComponent<Rigidbody>();
        mainCam = localPlayer.transform.GetChild(0).GetChild(0);
    }
    public void ResetToDefaultValues()
    {
        roundTimeIncrease = defaultRoundTimeIncrease;
        roundTime = defaultRoundTime;
        currentRound = 0;
    }

    public void SceneSwitch(int index)
    {
        runner.SetActiveScene(index);
    }

    public void OnGameStart()
    {
        gameOngoing = true;
        GameStart.Invoke();
        ConnectionManager.Instance.runner.SessionInfo.IsOpen = false;
        OnCountDownStart();
    }
    private void OnCountDownStart()
    {
        if (gameOngoing)
        {
            localRigid.isKinematic = true;
            CountDownStart.Invoke();

            StartCoroutine(PreRoundCountDown());
        }
    }
    private void OnCountDownEnd()
    {
        if (gameOngoing)
        {
            CountDownEnd.Invoke();
            localRigid.isKinematic = false;

            OnRoundStart();
        }
    }
    private void OnRoundStart()
    {
        if (gameOngoing)
        {
            currentRound++;
            RoundStart.Invoke();
            timer.SetTimer(roundTime);
        }
    }
    private void OnRoundEnd()
    {
        if (gameOngoing)
        {
            RoundEnd.Invoke();
            roundTime += roundTimeIncrease;
            OnRoundLateEnd();
        }
    }
    private void OnRoundLateEnd()
    {
        if (gameOngoing)
        {
            RoundLateEnd.Invoke();

            if (currentRound < amountOfRounds)
                OnCountDownStart();
            else
                OnGameEnd();
        }
    }
    private IEnumerator PreRoundCountDown()
    {
        if (gameOngoing)
        {
            yield return new WaitForSeconds(roundCountDownTime);
            OnCountDownEnd();
        }
    }
    private void OnGameEnd()
    {
        gameOngoing = false;
        GameEnd.Invoke();
    }
}
