using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class LeaderBoardCanvas : MonoBehaviour
{

    [SerializeField] private CompanyManager cManager;

    [SerializeField] private TextMeshProUGUI[] companyName;
    [SerializeField] private TextMeshProUGUI[] companyPoints;

    private Dictionary<string, int> rankDict;

    // Start is called before the first frame update
    void Start()
    {
        rankDict = new Dictionary<string, int> {
            {"water", 0},
            {"gas", 0},
            {"data", 0},
            {"sewage", 0},
            {"power", 0}
        };
    }

    public void SendPLayerData(PlayerData player)
    {
        rankDict.Add(player.company, player.points);
    }

    // Update is called once per frame
    public void UpdateLeaderBoard(PlayerData player)
    {
        rankDict[player.company] = player.points;
    }

    public void DisplayLeaderBoard()
    {
        rankDict = rankDict.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        int i = 0;

        foreach(var company in rankDict)
        {
            companyName[i].text = company.Key;
            companyPoints[i++].text = company.Value.ToString();
        }
    }
}
