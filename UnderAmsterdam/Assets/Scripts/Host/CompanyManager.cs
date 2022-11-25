using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;


public class CompanyManager : MonoBehaviour
{
    public static CompanyManager Instance;
    private ConnectionManager cManager;

    // All companies that are left, after players have been given a company
    private List<string> availableCompanies = new List<string> { "water", "gas", "data", "sewage", "power" };
    // History of all the companies a player has had
    private Dictionary<PlayerRef, List<string>> playerHistory = new Dictionary<PlayerRef, List<string>>();
    // Empty PlayerRef for the above dictionary
    public PlayerRef emptyPlayer = new();
    // Dictionary that keeps track what player has what company at the moment
    public Dictionary<string, PlayerRef> _companies;

    
    void Start(){
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        cManager = GetComponent<ConnectionManager>();
        Gamemanager.Instance.RoundEnd.AddListener(ResetCompanies);
        Gamemanager.Instance.RoundStart.AddListener(loadSend);
        _companies = new Dictionary<string, PlayerRef> {
    {"water", emptyPlayer},
    {"gas", emptyPlayer},
    {"data", emptyPlayer},
    {"sewage", emptyPlayer},
    {"power", emptyPlayer}
        };
    }

    string GetCompany(PlayerRef player) {
        if (availableCompanies.Count > 0) {
            string myCompany;
            int randomCompany;
            
            do { 
                randomCompany = Random.Range(0, availableCompanies.Count);
                myCompany = availableCompanies[randomCompany];
                if (playerHistory[player].Count == _companies.Count)
                    break;
            } while(playerHistory[player].Contains(myCompany));
            // Check if player has had this company already
            if (playerHistory[player].Count < _companies.Count)
            {
                // Add this company to player's history, so we don't see it again
                playerHistory[player].Add(myCompany);
                // Add player to company
                _companies[myCompany] = player;
                // Remove random company from available company list, so we don't have 2 players in same company
                availableCompanies.RemoveAt(randomCompany);
                return myCompany;
                // If there are still companies left we haven't had yet, do this function again until we return a company that wasn't given yet
            }
            Debug.LogError("Player has been through all companies, game should have ended");
            return "Empty";
        }
        Debug.LogError("No companies available");
        return "Empty";
    }

    public void loadSend() {
        foreach(var player in cManager._spawnedUsers) {
            if (!playerHistory.ContainsKey(player.Key)) {
                playerHistory.Add(player.Key, new List<string>());
            }
            SendCompany(player.Key, player.Value);
        }
    }

    // Function to send company to the correct player
    private void SendCompany(PlayerRef targetPlayer, NetworkObject player) {
        string sentCompany = GetCompany(targetPlayer);
        // Grab the playerdata of the player we want to send the company to
        player.gameObject.GetComponent<PlayerData>().ReceiveCompany(sentCompany);
    }

    public void ResetCompanies() {
        // Reset given companies
        availableCompanies = new List<string>{"water","gas","data","sewage","power"};
        _companies = new Dictionary<string, PlayerRef> {
    {"water", emptyPlayer},
    {"gas", emptyPlayer},
    {"data", emptyPlayer},
    {"sewage", emptyPlayer},
    {"power", emptyPlayer}};
    }
}
