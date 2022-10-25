using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class CompanyManager : MonoBehaviour
{
    [SerializeField]
    private List<int> totalCompanies = new List<int>{};

    int GetCompany() {
        int myCompany;
        myCompany = Random.Range(0, 6);
        totalCompanies.Remove(myCompany);
    return myCompany;
    }

    void ResetCompanies() {
        totalCompanies = new List<int>{};
    }

    void Update() {
        if(Input.GetKeyDown("space"))
            GetCompany();
    }
}
