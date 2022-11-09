using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class CompanyManager : MonoBehaviour
{
    [SerializeField]
    private List<string> availableCompanies = new List<string> { "water", "gas", "data", "sewage", "power" };
    PlayerRef emptyPlayer = new();
    [SerializeField] public Dictionary<string, PlayerRef> _companies;
    private ConnectionManager cManager;
    
    void Start(){
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
            int randomCompany = Random.Range(0, availableCompanies.Count);
            string myCompany = availableCompanies[randomCompany];
            // Add player to company
            _companies[myCompany] = player;
            // Remove random company from available company list, so we don't have 2 players in same company
            availableCompanies.RemoveAt(randomCompany);
            return myCompany;
        }
        return "Empty";
    }

    public void loadSend() {
        foreach(var player in cManager._spawnedUsers) {
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
