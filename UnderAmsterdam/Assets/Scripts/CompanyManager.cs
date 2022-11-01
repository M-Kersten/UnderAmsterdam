using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class CompanyManager : MonoBehaviour
{
    [SerializeField]
    private List<string> availableCompanies { get; set; }

    void Awake()
    {
        availableCompanies = new List<string> { "water", "gas", "data", "sewage", "power" };
    }

    string GetCompany() {
        if (availableCompanies.Count >= 0) {
            int randomCompany = Random.Range(0, availableCompanies.Count);
            string myCompany = availableCompanies[randomCompany];
            // Remove random company from company list, so we don't have 2 players in same company
            availableCompanies.RemoveAt(randomCompany);
            Debug.Log(randomCompany + " " + myCompany + " " + availableCompanies.Count);
            return myCompany;
        }
        return "Empty";
    }

    public void SendCompany(PlayerRef targetPlayer, NetworkObject player) {
        string sentCompany = GetCompany();
        player.gameObject.GetComponent<PlayerData>().RPC_ReceiveCompany(targetPlayer, sentCompany);
        Debug.Log("Company " + sentCompany + " Sent to " + targetPlayer.PlayerId);
    }

    void ResetCompanies() {
        // Reset given companies
        availableCompanies = new List<string>{"water","gas","data","sewage","power"};
    }
}
