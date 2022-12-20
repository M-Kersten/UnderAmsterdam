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
    private bool host;

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
        Gamemanager.Instance.RoundLateEnd.AddListener(EmptyTeamWork);
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
        if (!host) {
            host = Gamemanager.Instance.amIServer;
            Debug.Log("HOST");
        }
        // if both players are free, add them into contracts
        if (CheckFree(company1) && CheckFree(company2))
        {
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
            Debug.Log("company is Key: " + company + " other company: " + _companyContracts[company]);
            // Am I the Key? Return the value, so we know who we worked with
            return _companyContracts[company];
        } else if(_companyContracts.ContainsValue(company))
        {
            // Am I the Value? Check what Key belongs to me and return that, so we know who we worked with
            foreach(var myCompany in _companyContracts)
            {
               if (myCompany.Value == company) {
                Debug.Log("Other company: " + myCompany.Key + " my Company is Value: " + myCompany.Value);
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
            Debug.Log("Company: " + company);
    }

    private void CheckTeamwork()
    {
        if (_doneCompanies.Count > 1) {
            foreach(var company in _doneCompanies) { // go through all companies
            // Check if other company is done
                    Debug.Log("COMPANY: " + company.Key);
                    Debug.Log("Did I finish? " + _doneCompanies[company.Key]);
                    Debug.Log("Did my teammate finish? " + _doneCompanies[CheckMyCompany(company.Key)]);
                if(CheckMyCompany(company.Key) != null && _doneCompanies[CheckMyCompany(company.Key)]){
                    Debug.Log("BONUS TIME");
                  Gamemanager.Instance.pManager.TeamworkBonus(company.Key);
                }
            }
        }
    }

    public void EmptyTeamWork()
    {
        // Empty all contracts
        _companyContracts = new Dictionary<string, string>();

        _doneCompanies = new Dictionary<string, bool>();

    }
}
