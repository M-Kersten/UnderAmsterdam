using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class CompanyManager : MonoBehaviour
{
    [SerializeField]
    private List<string> availableCompanies = new List<string>{"water","gas","data","sewage","power"};

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

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AssignCompany(NetworkObject player) {
        player.gameObject.GetComponent<PlayerData>().company = GetCompany();
    }

    void ResetCompanies() {
        availableCompanies = new List<string>{"water","gas","data","sewage","power"};
    }
}
