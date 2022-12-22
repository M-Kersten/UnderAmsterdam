using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion.XR.Host;
using Fusion;

public class ScoreBoard : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI[] PlayerTMP;
    [SerializeField] private TextMeshProUGUI roundTMP;
    [SerializeField] private ConnectionManager cManager;
    [SerializeField] private bool perRound;

    public Dictionary<string, int> rankDict;
    private Dictionary<string, PlayerRef> savedCompanies;
    private int[] startPoints;
    private int round = 0;

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SendData(PlayerData player, int startPoint)
    {
        rankDict.Add(player.company, player.points - startPoint);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DisplayData()
    {
        round++;
        DisplayLeaderBoard();
    }

    public override void Spawned()
    {

        cManager = FindObjectOfType<ConnectionManager>();
        if (HasStateAuthority)
        {
            if (perRound) Gamemanager.Instance.RoundStart.AddListener(GetStartPoints);
            Gamemanager.Instance.RoundEnd.AddListener(UpdateLeaderBoard);
            Gamemanager.Instance.GameEnd.AddListener(dontDestroy);
            ConnectionManager.Instance.A4Loaded.AddListener(warpPlayers);
        }
    }

    void dontDestroy()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        rankDict = new Dictionary<string, int>();
        startPoints = new int[] { 0, 0, 0, 0, 0 };
    }

    private void GetStartPoints()
    {
        int i = 0;
        foreach (var company in CompanyManager.Instance._companies)
        {
            if (company.Value != PlayerRef.None) startPoints[i++] = cManager._spawnedUsers[CompanyManager.Instance._companies[company.Key]].GetComponent<PlayerData>().points;
        }
    }

    private void UpdateLeaderBoard()
    {
        int i = 0;
        //Updates the dictionnary;
        foreach (var company in CompanyManager.Instance._companies)
        {
            if (company.Value == PlayerRef.None) continue;
            PlayerData player = cManager._spawnedUsers[CompanyManager.Instance._companies[company.Key]].GetComponent<PlayerData>();
            RPC_SendData(player, startPoints[i++]);
        }

        //Sorts ScoreBoard
        rankDict = rankDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        RPC_DisplayData();
    }

    private void DisplayLeaderBoard()
    {
        int i = 0;

        //Displays points for each player in the dictionnary
        foreach (var player in rankDict)
        {
            PlayerTMP[i++].text = player.Key + " : " + player.Value.ToString();
        }

        //Displays Header
        if (round < 5)
        {
            roundTMP.text = "Round " + round.ToString();
            rankDict.Clear();
        }
        else
        {
            roundTMP.text = "GAMEOVER";
            savedCompanies = CompanyManager.Instance._companies;
        }
    }

    void warpPlayers()
    {
        NetworkObject nObject;

        //Gamemanager.Instance.lPlayerCC.gameObject.transform.position = new Vector3(0f, 3f, 5.5f);
        //Gamemanager.Instance.lPlayerCC.gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
        
        if (rankDict.Count == 0) return;

        nObject = cManager._spawnedUsers[savedCompanies[rankDict.ElementAt(0).Key]];
        nObject.gameObject.transform.position = new Vector3(0f, 3f, 5.5f);
        nObject.gameObject.transform.eulerAngles = new Vector3(0, 180, 0);

        if (rankDict.Count == 1) return;

        nObject = cManager._spawnedUsers[savedCompanies[rankDict.ElementAt(1).Key]];
        nObject.gameObject.transform.position = new Vector3(-2.5f, 2.5f, 4.5f);
        nObject.gameObject.transform.eulerAngles = new Vector3(0, 180, 0);

        if (rankDict.Count == 2) return;

        nObject = cManager._spawnedUsers[savedCompanies[rankDict.ElementAt(2).Key]];
        nObject.gameObject.transform.position = new Vector3(2f, 2.5f, 3.5f);
        nObject.gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
    }

}