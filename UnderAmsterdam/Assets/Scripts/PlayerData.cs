using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerData : MonoBehaviour
{
    PlayerRef player;
    int playerId;
    public string company;
    string prevCompany;
    int points;

    void Start()
    {
        playerId = player.PlayerId;
    }
}
