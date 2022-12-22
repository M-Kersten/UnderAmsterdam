using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class ScoreBoard : NetworkBehaviour
{

    [SerializeField] private TextMeshProUGUI[] PlayerTMP;
    [SerializeField] private TextMeshProUGUI roundTMP;

    [Networked(OnChanged = nameof(onSharedData))]
    private PlayerData sharedPlayer { get; set; }

    public Dictionary<string, int> rankDict;

    private int round = 0;

    // Start is called before the first frame update
    void Start()
    {
        rankDict = new Dictionary<string, int>();
        //Gamemanager.Instance.RoundStart.AddListener(DisplayLeaderBoard);
    }

    static void onSharedData(Changed<ScoreBoard> changed)
    {
        changed.Behaviour.SendPlayerData(changed.Behaviour.sharedPlayer);
    }

    public void SendPlayerData(PlayerData player)
    {
        //Get points from all players (in playerData)
        rankDict.Add("NickName"+rankDict.Count.ToString(), player.points + (int)Random.Range(-50f, 50f));
        if (rankDict.Count == 2) DisplayLeaderBoard();
    }

    private void DisplayLeaderBoard()
    {
        round++;

        //Sorting ScoreBoard
        rankDict = rankDict.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        int i = 0;

        Debug.Log(rankDict.Count.ToString());

        foreach (var player in rankDict)
        {
            Debug.Log(player.Key + player.Value.ToString());
            PlayerTMP[i++].text = player.Key + " : " + player.Value.ToString();
        }

        if (round < 5)
            roundTMP.text = "Round " + round.ToString();
        else
            roundTMP.text = "GAMEOVER";

        rankDict.Clear();
    }
}
