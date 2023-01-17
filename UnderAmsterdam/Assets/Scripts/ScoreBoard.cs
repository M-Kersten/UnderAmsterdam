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
    [SerializeField] private bool perRound;
    [SerializeField] private Transform[] podiumPipes;
    [SerializeField] private GameObject podium;
    [SerializeField] private GameObject container;

    private ConnectionManager cManager;
    private Dictionary<string, int> rankDict;
    private Dictionary<string, PlayerRef> savedCompanies;
    private int[] startPoints;

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SendData(PlayerData player, int startPoint)
    {
        rankDict.Add(player.company, player.points - startPoint);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DisplayData()
    {
        DisplayLeaderBoard();
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_WarpPlayers()
    {
        WarpPlayers();
    }

    public override void Spawned()
    {
        cManager = FindObjectOfType<ConnectionManager>();
        if (HasStateAuthority)
        {
            if (perRound) Gamemanager.Instance.RoundStart.AddListener(GetStartPoints);
            Gamemanager.Instance.GameEnd.AddListener(UpdateLeaderBoard);
            //Gamemanager.Instance.GameLateEnd.AddListener(RPC_WarpPlayers);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rankDict = new Dictionary<string, int>();
        startPoints = new int[] { 0, 0, 0, 0, 0 };
        podium.SetActive(false);
        container.SetActive(false);
    }

    private void GetStartPoints()
    {
        int i = 0;
        foreach (var company in CompanyManager.Instance._companies)
        {
            if (company.Value != PlayerRef.None) startPoints[i++] = cManager._spawnedUsers[CompanyManager.Instance._companies[company.Key]].GetComponent<PlayerData>().points;
        }
    }

    public void UpdateLeaderBoard()
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
        container.SetActive(true);
        int i = 0;

        //Displays points for each player in the dictionnary
        foreach (var player in rankDict)
        {
            PlayerTMP[i++].text = player.Key + " : " + player.Value.ToString();
        }

        savedCompanies = CompanyManager.Instance._companies;
    }

    public void WarpPlayers()
    {
        podium.SetActive(true);
        for (int i = 0; i < podiumPipes.Length && i < rankDict.Count; i++)
        {
            if (cManager.localPlayerRef == savedCompanies[rankDict.ElementAt(i).Key])
            {
                Gamemanager.Instance.lPlayerCC.enabled = false;
                Gamemanager.Instance.lPlayerCC.gameObject.transform.position = podiumPipes[i].position + new Vector3(0, 2f - i/2f, -0.5f);
                Gamemanager.Instance.lPlayerCC.gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
                Gamemanager.Instance.lPlayerCC.enabled = true;
                return;
            }
        }

        /*Gamemanager.Instance.lPlayerCC.enabled = false;
        Gamemanager.Instance.lPlayerCC.gameObject.transform.position = new Vector3(1.02216411f, 4.0f, 5f);
        Gamemanager.Instance.lPlayerCC.gameObject.transform.eulerAngles = new Vector3(0, 90, 0);
        Gamemanager.Instance.lPlayerCC.enabled = true;*/

    }

}