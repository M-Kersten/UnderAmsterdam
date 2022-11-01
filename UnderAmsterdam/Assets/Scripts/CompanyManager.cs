using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class CompanyManager : MonoBehaviour
{
    [SerializeField]
    private List<string> availableCompanies { get; set; }
    private List<PlayerRef> test1 { get; set; }
    private List<NetworkObject> test2 { get; set; }

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

    public void StorePlayers(PlayerRef targetPlayer, NetworkObject player)
    {
        test1.Add(targetPlayer);
        test2.Add(player);
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            foreach (PlayerRef player in test1)
            {
                foreach(NetworkObject nObject in test2)
                {
                    nObject.gameObject.GetComponent<PlayerData>().RPC_ReceiveCompany(player, GetCompany());
                }
            }
        }
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
