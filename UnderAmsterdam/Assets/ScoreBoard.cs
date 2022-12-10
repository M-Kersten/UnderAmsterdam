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

    private List<string> companies = new List<string> { "water", "gas", "data", "sewage", "power" };
    private Dictionary<string, int> rankDict;
    private int round = 0;

    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this.gameObject);
        rankDict = new Dictionary<string, int>();
        Gamemanager.Instance.RoundStart.AddListener(DisplayLeaderBoard);
    }

    static void onSharedData(Changed<ScoreBoard> changed)
    {
        changed.Behaviour.SendPlayerData(changed.Behaviour.sharedPlayer);
    }

    public void SendPlayerData(PlayerData player)
    {
        sharedPlayer = player;
        //Gets points from all players (in playerData)
        rankDict.Add("NickName" + rankDict.Count.ToString(), player.points + (int)Random.Range(-50f, 50f));
    }

    private void UpdateLeaderBoard()
    {
        round++;
        //Updates the dictionnary;
        foreach (string company in companies)
        {
            PlayerData player = cManager._spawnedUsers[CompanyManager.Instance._companies[company]].GetComponent<PlayerData>();
            SendPlayerData(player);
        }
        //Sorts ScoreBoard
        rankDict = rankDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
    }

    private void DisplayLeaderBoard()
    {
        UpdateLeaderBoard();

        int i = 0;

        //Displays points for each player in the dictionnary
        foreach (var player in rankDict)
        {
            Debug.Log(player.Key + player.Value.ToString());
            PlayerTMP[i++].text = player.Key + " : " + player.Value.ToString();
        }

        //Displays Header
        if (round < 5)
            roundTMP.text = "Round " + round.ToString();
        else
            roundTMP.text = "GAMEOVER";

        rankDict.Clear();
    }
}