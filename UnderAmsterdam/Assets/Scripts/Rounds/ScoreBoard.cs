using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion.XR.Host;
using Fusion;
using Fusion.Addons.ConnectionManagerAddon;

public class ScoreBoard : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI[] PlayerTMP;
    [SerializeField] private TextMeshProUGUI roundTMP;
    [SerializeField] private bool perRound;
    [SerializeField] private Transform[] podiumPipes;
    [SerializeField] private GameObject podium;
    [SerializeField] private GameObject container;

    private ConnectionManager cManager;
    private Dictionary<int, int> rankDict = new();
    private Dictionary<int, PlayerRef> savedCompanies = new();

    public override void Spawned()
    {
        cManager = FindObjectOfType<ConnectionManager>();
        if (HasStateAuthority) 
            Gamemanager.Instance.GameEnd.AddListener(UpdateLeaderBoard);
    }

    // Start is called before the first frame update
    void Start()
    {
        podium.SetActive(false);
        container.SetActive(false);
    }

    public void UpdateLeaderBoard()
    {
        //Updates the dictionnary;
        foreach (var company in CompanyManager.Instance.Companies)
        {
            if (company.Value == PlayerRef.None) 
                continue;
            
            var player = cManager.SpawnedUsers[CompanyManager.Instance.Companies[company.Key]].GetComponent<PlayerData>();
            RPC_SendData(player);
        }

        foreach (var company in CompanyManager.Instance.Companies) RPC_SendPlayersCompany(company.Key, company.Value);

        RPC_DisplayData();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SendData(PlayerData player)
    {
        if(!rankDict.ContainsKey(player.Company))
            rankDict.Add(player.Company, player.points);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_DisplayData()
    {
        DisplayLeaderBoard();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SendPlayersCompany(int company, PlayerRef player)
    {
        savedCompanies.TryAdd(company, player);
    }

    private void DisplayLeaderBoard()
    {
        //Sorts ScoreBoard
        rankDict = rankDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        container.SetActive(true);
        
        int i = 0;
        foreach (var player in rankDict)
        {
            PlayerTMP[i].text = Enum.GetValues(typeof(CompanyType)).GetValue(player.Key) + " : " + player.Value;
            Debug.Log($"player: {Enum.GetValues(typeof(CompanyType)).GetValue(player.Key)} placed at {i}");
            i++;
        }
    }

    public void WarpPlayers()
    {
        podium.SetActive(true);

        for (int i = 0; i < podiumPipes.Length && i < rankDict.Count; i++)
        {
            if (cManager.ConnectionSettings.LocalPlayerRef == savedCompanies[rankDict.ElementAt(i).Key])
            {
                Gamemanager.Instance.hardwareRig.Teleport(podiumPipes[i].position + new Vector3(0, 3.5f - i, -0.5f));
                Gamemanager.Instance.localRigid.gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
                return;
            }
        }
    }
}