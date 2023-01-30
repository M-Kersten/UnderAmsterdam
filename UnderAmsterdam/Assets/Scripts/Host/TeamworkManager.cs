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
    private Dictionary<PlayerRef, PlayerRef> _donePlayers = new Dictionary<PlayerRef, PlayerRef>();
    private CompanyManager compManager;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this.gameObject);
        }

        compManager = CompanyManager.Instance;
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
        return "NULL";
    }

    public void CompanyDone(string company) {
        if (!_doneCompanies.ContainsKey(company))
        {
            _doneCompanies.Add(company, true);

            // If other player is done + I am not a key or Value ( doing this cause company gets reset before we can give points )
            if(compManager._companies.ContainsKey(CheckMyCompany(company)))
            {
                if (_donePlayers.ContainsKey(compManager._companies[CheckMyCompany(company)]) && !_donePlayers.ContainsKey(compManager._companies[company]))
                    _donePlayers[compManager._companies[CheckMyCompany(company)]] = compManager._companies[company];
                else if (!_donePlayers.ContainsValue(compManager._companies[company]))
                    _donePlayers.Add(compManager._companies[company], compManager.emptyPlayer);
            }
        }
    }

    private void CheckTeamwork()
    {
        foreach(var player in _donePlayers)
        {
            if(player.Value != compManager.emptyPlayer)
            {
                // Add points to both players in the contract
                Gamemanager.Instance.pManager.TeamworkBonus(player.Key);
                Gamemanager.Instance.pManager.TeamworkBonus(player.Value);
            }
        }
    }

    public void EmptyTeamWork()
    {
        // Empty all contracts
        _companyContracts = new Dictionary<string, string>();

        _doneCompanies = new Dictionary<string, bool>();
        _donePlayers = new Dictionary<PlayerRef, PlayerRef>();
    }
}
