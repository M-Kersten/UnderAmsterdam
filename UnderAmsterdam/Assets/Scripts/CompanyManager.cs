using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class CompanyManager : MonoBehaviour
{
    [SerializeField]
    private List<string> availableCompanies = new List<string> { "water", "gas", "data", "sewage", "power" };
    [SerializeField] public Dictionary<string, int> _companies = new Dictionary<string, int> {
	{"water", 0},
	{"gas", 0},
	{"data", 0},
    {"sewage", 0},
    {"power", 0}
};
    private ConnectionManager cManager;
    
    void Start(){
        cManager = GetComponent<ConnectionManager>();
        Gamemanager.Instance.RoundEnd.AddListener(ResetCompanies);
        Gamemanager.Instance.RoundStart.AddListener(loadSend);
    }

    string GetCompany(PlayerRef player) {
        if (availableCompanies.Count > 0) {
            int randomCompany = Random.Range(0, availableCompanies.Count);
            string myCompany = availableCompanies[randomCompany];
            // Add player to company
            _companies[myCompany] = player.PlayerId;
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
        _companies = new Dictionary<string, int> {
            {"water", 0},
            {"gas", 0},
            {"data", 0},
            {"sewage", 0},
            {"power", 0}
        };
    }
}
