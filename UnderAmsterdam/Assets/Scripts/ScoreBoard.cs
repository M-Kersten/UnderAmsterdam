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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SendData(PlayerData player)
    {
        if(!rankDict.ContainsKey(player.company))
        rankDict.Add(player.company, player.points);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DisplayData()
    {
        DisplayLeaderBoard();
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SendPlayersCompany(string company, PlayerRef player)
    {
        if (!savedCompanies.ContainsKey(company))
        savedCompanies.Add(company, player);
    }

    public override void Spawned()
    {
        cManager = FindObjectOfType<ConnectionManager>();
        if (HasStateAuthority) Gamemanager.Instance.GameEnd.AddListener(UpdateLeaderBoard);
    }

    // Start is called before the first frame update
    void Start()
    {
        rankDict = new Dictionary<string, int>();
        savedCompanies = new Dictionary<string, PlayerRef>();
        podium.SetActive(false);
        container.SetActive(false);
    }

    public void UpdateLeaderBoard()
    {
        //Updates the dictionnary;
        foreach (var company in CompanyManager.Instance._companies)
        {
            if (company.Value == PlayerRef.None) continue;
            PlayerData player = cManager._spawnedUsers[CompanyManager.Instance._companies[company.Key]].GetComponent<PlayerData>();
            RPC_SendData(player);
        }

        foreach (var company in CompanyManager.Instance._companies) RPC_SendPlayersCompany(company.Key, company.Value);

        RPC_DisplayData();
    }

    private void DisplayLeaderBoard()
    {
        //Sorts ScoreBoard
        rankDict = rankDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        container.SetActive(true);
        int i = 0;

        //Displays points for each player in the dictionnary
        foreach (var player in rankDict) PlayerTMP[i++].text = player.Key + " : " + player.Value.ToString();
    }

    public void WarpPlayers()
    {
        podium.SetActive(true);
        for (int i = 0; i < podiumPipes.Length && i < rankDict.Count; i++)
        {
            if (cManager.localPlayerRef == savedCompanies[rankDict.ElementAt(i).Key])
            {
                Gamemanager.Instance.localRigid.gameObject.transform.position = podiumPipes[i].position + new Vector3(0, 3.5f - i, -0.5f);
                Gamemanager.Instance.localRigid.gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
                return;
            }
        }
    }
}