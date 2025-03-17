using UnityEngine;
using Fusion.XR.Host;
using Fusion;
using UnityEngine.Events;
using System.Collections;
using Fusion.Addons.ConnectionManagerAddon;
using Fusion.XR.Host.Rig;
using UnityEngine.SceneManagement;

public class Gamemanager : MonoBehaviour
{
    public static Gamemanager Instance;
    
    public UnityEvent GameStart, RoundStart, RoundEnd, RoundLateEnd, GameEnd, CountDownStart, CountDownEnd;

    public ConnectionManager ConnectionManager;
    public PlayerData networkData;

    public GameObject localPlayer;
    public LocalData localData;
    public PlayerInputHandler playerInputHandler;
    public Rigidbody localRigid;
    public HardwareRig hardwareRig;
    public Transform mainCam;

    public float roundTimeIncrease = 10;
    public float roundTime = 45;

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
        hardwareRig = localPlayer.GetComponent<HardwareRig>();
        mainCam = localPlayer.transform.GetChild(0).GetChild(0);
    }
    
    public void ResetToDefaultValues()
    {
        roundTimeIncrease = defaultRoundTimeIncrease;
        roundTime = defaultRoundTime;
        currentRound = 0;
    }

    public void OnGameStart()
    {
        gameOngoing = true;
        GameStart.Invoke();
        ConnectionManager.runner.SessionInfo.IsOpen = false;
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
            Debug.Log("Countdown end");
            CountDownEnd.Invoke();
            localRigid.isKinematic = false;

            OnRoundStart();
        }
    }
    
    private void OnRoundStart()
    {
        if (gameOngoing)
        {
            Debug.Log("Round start");
            currentRound++;
            RoundStart.Invoke();
            timer.SetTimer(roundTime);
        }
    }
    
    private void OnRoundEnd()
    {
        if (gameOngoing)
        {
            Debug.Log("Round end");
            RoundEnd.Invoke();
            roundTime += roundTimeIncrease;
            OnRoundLateEnd();
        }
    }
    
    private void OnRoundLateEnd()
    {
        if (gameOngoing)
        {
            Debug.Log("Route late end");
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
            Debug.Log("Countdown start");
            yield return new WaitForSeconds(roundCountDownTime);
            OnCountDownEnd();
        }
    }
    
    private void OnGameEnd()
    {
        gameOngoing = false;
        GameEnd.Invoke();
    }
    
    public void TeleportToStartPosition()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;
        
        if (localData) 
           localData.gameObject.name = "LocalPlayerSession";
        
        var tPosition = new Vector3(0.0f, 0.0f, 1.65285861f);
        Debug.Log($"Teleporting player to: {tPosition.ToString()}");
        hardwareRig.Teleport(tPosition);
        localRigid.GetComponent<Animator>().Play("ReverseVisionFadeLocal", 0);
    }
    
    public void ShutdownPlayer(NetworkRunner runner)
    {
        localRigid.GetComponent<Animator>().Play("ReverseVisionFadeLocal", 0);
        gameOngoing = false;
        if (runner.IsServer)
            ConnectionManager.SpawnedUsers.Remove(networkData.GetComponent<NetworkObject>().InputAuthority);

        localData.transform.position = new Vector3(0, 0, 0);
        SceneManager.LoadScene(0);

        gameObject.SetActive(false);
        gameObject.SetActive(true);
        ResetToDefaultValues();
    }
}
