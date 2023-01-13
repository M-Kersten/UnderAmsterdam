using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamworkManager : MonoBehaviour
{
    public static TeamworkManager Instance;
    private Dictionary<string, string> _companyContracts = new Dictionary<string, string>();
    private Dictionary<string, bool> _doneCompanies = new Dictionary<string, bool>();

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this.gameObject);
        }

        Gamemanager.Instance.RoundLateEnd.AddListener(CheckTeamwork);
        Gamemanager.Instance.RoundStart.AddListener(EmptyTeamWork);
    }

    private bool CheckFree(string company)
    {
        // Check if player is free to work with someone
        if (!_companyContracts.ContainsKey(company) && !_companyContracts.ContainsValue(company))
        {
            return true;
        }
        return false;
    }


    public bool AddTeamWork(string company1, string company2)
    {
        // if both players are free, add them into contracts
        if (CheckFree(company1) && CheckFree(company2))
        {
            Debug.Log("TEAMWORK: ADDING CONTRACT " + company1 + " " + company2);
            _companyContracts.Add(company1, company2);
            return true;
        }
        return false;
    }

    private string CheckMyCompany(string company)
    {
        // Check if I have a contract and if I am the Key or the Value
        if (_companyContracts.ContainsKey(company))
        {
            // Am I the Key? Return the value, so we know who we worked with
            return _companyContracts[company];
        } else if(_companyContracts.ContainsValue(company))
        {
            // Am I the Value? Check what Key belongs to me and return that, so we know who we worked with
            foreach(var myCompany in _companyContracts)
            {
               if (myCompany.Value == company) {
                return myCompany.Key;
               }
                    //return myCompany.Key;
            }
        }
        // I didn't work with anyone
        return null;
    }

    public void CompanyDone(string company) {
            _doneCompanies.Add(company, true);
        Debug.Log("TEAMWORK: ADDING COMPANY " + company);
    }

    private void CheckTeamwork()
    {
            foreach(var company in _doneCompanies) { // go through all companies
            // Check if other company is done
                if(CheckMyCompany(company.Key) != null && _doneCompanies.ContainsKey(CheckMyCompany(company.Key))){
                  Gamemanager.Instance.pManager.TeamworkBonus(company.Key);
                }
            }
    }

    public void EmptyTeamWork()
    {
        Debug.Log("TEAMWORK: START EMPTYING..");
        // Empty all contracts
        _companyContracts = new Dictionary<string, string>();

        _doneCompanies = new Dictionary<string, bool>();

        Debug.Log("TEAMWORK: DONE EMPTYING");

    }
}
