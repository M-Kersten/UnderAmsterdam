using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class CompanyManager : MonoBehaviour
{
    [SerializeField]
    private List<int> availableCompanies = new List<int>{};

    int GetCompany() {
        int myCompany;
        myCompany = Random.Range(0, 6);
        availableCompanies.Remove(myCompany);
    return myCompany;
    }

    void ResetCompanies() {
        availableCompanies = new List<int>{0,1,2,3,4,5};
    }

    void Update() {
        if(Input.GetKeyDown("space"))
            GetCompany();

        if(Input.GetKeyDown("k"))
            ResetCompanies();
    }
}
