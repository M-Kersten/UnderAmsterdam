using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderBoardCanvas : MonoBehaviour
{

    [SerializeField] private CompanyManager cManager;

    [SerializeField] private TextMeshProUGUI[] companyName;
    [SerializeField] private TextMeshProUGUI[] companyPoints;

    private Dictionary<string, int> rankDict;
    private string[] rankedCompanies;
    private int numberOfCompanies = 5;

    // Start is called before the first frame update
    void Start()
    {
        rankedCompanies = new string[numberOfCompanies];
        rankDict = new Dictionary<string, int> {
            {"water", 0},
            {"gas", 0},
            {"data", 0},
            {"sewage", 0},
            {"power", 0}
        };
    }

    // Update is called once per frame
    public void UpdateLeaderBoard(PlayerData player)
    {
        rankDict[player.company] = player.points;

        if (SortLeaderBoard(player))
        {

        }

        for (int i = 0; i < cManager._companies.Count; i++)
        {

        }
    }

    private bool SortLeaderBoard(PlayerData player)
    {
        int initialRank = -1;

        for (int i = 0; player.company != rankedCompanies[i]; i++) initialRank = i;

        for (int i = 0; i < cManager._companies.Count; i++)
        {
            if (rankedCompanies[i] != player.company && rankDict[rankedCompanies[i]] <= player.points)
            {
                for (int j = initialRank; j != i; j += initialRank > i ? 1 : -1) rankedCompanies[j] = rankedCompanies[j + initialRank > i ? 1 : -1];
                rankedCompanies[i] = player.company;

                return (i == initialRank);
            }
        }
        return false;
    }
}
