using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamworkManager : MonoBehaviour
{
    public static TeamworkManager Instance;
    private Dictionary<string, string> _companyContracts;
    private Dictionary<string, bool> _doneCompanies;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this.gameObject);
        }

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

    public void AddTeamWork(string company1, string company2)
    {
        // if both players are free, add them into contracts
        if (CheckFree(company1) && CheckFree(company2))
        {
            _companyContracts.Add(company1, company2);
        }
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
               if (myCompany.Value == company)
                    return myCompany.Key;
            }
        }
        // I didn't work with anyone
        return null;
    }

    void Update() {
        if (Input.GetKeyDown("space")) {
            foreach(var team in _companyContracts) 
            {
            Debug.Log("SIGNED CONTRACTS: " + team.Key + " " + team.Value);

            }
        }
    }

    public void CheckTeamwork(string company)
    {
        // Show that I am done
        _doneCompanies.Add(company, true);
        // Check if I had a contract and if my contract buddy is done
        if (_doneCompanies[CheckMyCompany(company)] && CheckMyCompany(company) != null)
        {
            // Add teamwork bonus to me
            //Gamemanager.Instance.pManager.TeamworkBonus(company);
        }
    }

    private void EmptyTeamWork() {
        _doneCompanies.Clear();
        _companyContracts.Clear();
    }
}
