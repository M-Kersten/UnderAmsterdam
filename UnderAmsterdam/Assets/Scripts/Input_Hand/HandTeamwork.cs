using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTeamwork : MonoBehaviour
{
    [SerializeField] private PlayerData myData;
    [SerializeField] private GameObject particle;

    private void OnTriggerEnter(Collider other){
        if (other.gameObject.layer == 14 && other.gameObject.GetComponent<HandTeamwork>().myData.company != myData.company){ // Teamwork layer
            if (TeamworkManager.Instance.AddTeamWork(myData.company, other.gameObject.GetComponent<HandTeamwork>().myData.company)) {
                Debug.Log(myData.company + " <3 " + other.gameObject.GetComponent<HandTeamwork>().myData.company);
            }
        }
    }
}
