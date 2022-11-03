using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class CompanyManager : MonoBehaviour
{
    [SerializeField]
    private List<string> availableCompanies = new List<string> { "water", "gas", "data", "sewage", "power" };
    private ConnectionManager cManager;
    
    void Start(){
        cManager = GetComponent<ConnectionManager>();
    }

    string GetCompany() {
        if (availableCompanies.Count > 0) {
            int randomCompany = Random.Range(0, availableCompanies.Count);
            string myCompany = availableCompanies[randomCompany];
            // Remove random company from company list, so we don't have 2 players in same company
            availableCompanies.RemoveAt(randomCompany);
            Debug.Log(randomCompany + " " + myCompany + " " + availableCompanies.Count);
            return myCompany;
        }
        return "Empty";
    }

    public void Update() {
        if(Input.GetKeyDown("space"))
        loadSend();
    }

    public void loadSend() {
        foreach(var player in cManager._spawnedUsers) {
            SendCompany(player.Key, player.Value);
        }
    }
         //  foreach (PlayerRef player in test1)
         //  {
         //      foreach(NetworkObject nObject in test2)
         //      {
         //          nObject.gameObject.GetComponent<PlayerData>().RPC_ReceiveCompany(player, GetCompany());
         //          Debug.Log("Running lol");
         //      }
         //  }

    // Function to send company to the correct player
    public void SendCompany(PlayerRef targetPlayer, NetworkObject player) {
        string sentCompany = GetCompany();
        // Grab the playerdata of the player we want to send the company to
        player.gameObject.GetComponent<PlayerData>().RPC_ReceiveCompany(targetPlayer, sentCompany);
        Debug.Log("Company " + sentCompany + " Sent to " + targetPlayer.PlayerId);
    }

    void ResetCompanies() {
        // Reset given companies
        availableCompanies = new List<string>{"water","gas","data","sewage","power"};
    }
}
