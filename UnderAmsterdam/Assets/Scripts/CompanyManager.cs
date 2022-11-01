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
        return null;
    }

    void AssignTeam(PlayerRef player) {
       // player.GetComponent<PlayerData>().team = GetCompany();
    }

    void ResetCompanies() {
        availableCompanies = new List<string>{"water","gas","data","sewage","power"};
    }

    void Update() {
        if(Input.GetKeyDown("space"))
            GetCompany();

        if(Input.GetKeyDown("k"))
            ResetCompanies();
    }
}
