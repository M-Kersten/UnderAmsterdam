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

    [Networked(OnChanged = nameof(onSharedData))]
    private PlayerData sharedPlayer { get; set; }

    [Networked(OnChanged = nameof(onDisplayData))]
    private bool display { get; set; }

    private List<string> companies = new List<string> { "water", "gas", "data", "sewage", "power" };
    private Dictionary<string, int> rankDict;
    private int round = 0;

    public override void Spawned()
    {
        if (HasStateAuthority) Gamemanager.Instance.RoundEnd.AddListener(UpdateLeaderBoard);
    }

    // Start is called before the first frame update
    void Start()
    {
        rankDict = new Dictionary<string, int>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space")) UpdateLeaderBoard();
    }

    static void onSharedData(Changed<ScoreBoard> changed)
    {
        changed.Behaviour.SendPlayerData(changed.Behaviour.sharedPlayer);
    }

    static void onDisplayData(Changed<ScoreBoard> changed)
    {
        changed.Behaviour.DisplayLeaderBoard();
    }

    public void SendPlayerData(PlayerData player)
    {
        //sharedPlayer = player;
        //Gets points from all players (in playerData)
        Debug.Log(player.company + rankDict.Count.ToString() + HasStateAuthority.ToString());
        rankDict.Add(player.company + rankDict.Count.ToString(), player.points + (int)Random.Range(-50f, 50f));
    }

    private void UpdateLeaderBoard()
    {
        round++;
        //Updates the dictionnary;
        foreach (var company in CompanyManager.Instance._companies)
        {
            if (company.Value == PlayerRef.None) continue;
            PlayerData player = cManager._spawnedUsers[CompanyManager.Instance._companies[company.Key]].GetComponent<PlayerData>();
            SendPlayerData(player);
        }
        //Sorts ScoreBoard
        rankDict = rankDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        DisplayLeaderBoard();
    }

    private void DisplayLeaderBoard()
    {
        //display = true;

        int i = 0;

        //Displays points for each player in the dictionnary
        foreach (var player in rankDict)
        {
            PlayerTMP[i++].text = player.Key + " : " + player.Value.ToString();
        }

        //Displays Header
        if (round < 5)
            roundTMP.text = "Round " + round.ToString();
        else
            roundTMP.text = "GAMEOVER";

        rankDict.Clear();
        CompanyManager.Instance.ResetCompanies();
    }
}