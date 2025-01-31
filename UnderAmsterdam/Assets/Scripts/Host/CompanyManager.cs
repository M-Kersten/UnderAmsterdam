using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;
using Random = UnityEngine.Random;


public class CompanyManager : MonoBehaviour
{
    public static CompanyManager Instance;

    // All companies that are left, after players have been given a company
    private List<int> availableCompanies = new();
    // History of all the companies a player has had
    private Dictionary<PlayerRef, List<int>> playerHistory = new();
    // Empty PlayerRef for the above dictionary
    public PlayerRef emptyPlayer;
    // Dictionary that keeps track what player has what company at the moment
    public Dictionary<int, PlayerRef> Companies;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        Gamemanager.Instance.RoundLateEnd.AddListener(CheckForResetCompanies);
        Gamemanager.Instance.RoundStart.AddListener(loadSend);
        ResetCompanies();
    }

    int GetCompany(PlayerRef player) 
    {
        if (availableCompanies.Count > 0) 
        {
            int myCompany = -1;
            int randomCompany = 0;
            
            
            if (playerHistory[player].Count < Companies.Count)
            {
                // Keep randomizing company, until we find one that player hasn't had yet
                do 
                { 
                    // if there aren't any more companies left, quit the do-while loop to stop infinite looping
                    if (playerHistory[player].Count == Companies.Count)
                        break;
                    
                    randomCompany = Random.Range(0, availableCompanies.Count);
                    myCompany = availableCompanies[randomCompany];
                } 
                while(playerHistory[player].Contains(myCompany));

                // Add this company to player's history, so we don't see it again
                playerHistory[player].Add(myCompany);
                // Add player to company
                Companies[myCompany] = player;
                // Remove random company from available company list, so we don't have 2 players in same company
                availableCompanies.RemoveAt(randomCompany);
                return myCompany;
                // If there are still companies left we haven't had yet, do this function again until we return a company that wasn't given yet
            }
            return -1;
        }
        //Debug.LogError("No companies available");
        return -1;
    }

    public void loadSend() 
    {
        foreach(var player in Gamemanager.Instance.ConnectionManager.SpawnedUsers) 
        {
            if (!playerHistory.ContainsKey(player.Key)) 
            {
                playerHistory.Add(player.Key, new List<int>());
            }
            SendCompany(player.Key, player.Value);
        }
    }

    // Function to send company to the correct player
    private void SendCompany(PlayerRef targetPlayer, NetworkObject nObject) 
    {
        int sentCompany = GetCompany(targetPlayer);
        // Grab the playerdata of the player we want to send the company to
        nObject.gameObject.GetComponent<PlayerData>().ReceiveCompany(sentCompany);
    }

    private void CheckForResetCompanies()
    {
        if (Gamemanager.Instance.currentRound >= Gamemanager.Instance.amountOfRounds)
            return;

        ResetCompanies();
    }

    private void ResetCompanies()
    {
        availableCompanies = Enum.GetValues(typeof(CompanyType))
            .Cast<CompanyType>()
            .Select(e => (int)e).ToList();
        Companies = InitializePlayerRefDictionary(emptyPlayer);
    }
    
    private Dictionary<int, PlayerRef> InitializePlayerRefDictionary(PlayerRef playerRef)
    {
        return Enum.GetValues(typeof(CompanyType))
            .Cast<CompanyType>()
            .ToDictionary(company => (int)company, company => playerRef);
    }
}
