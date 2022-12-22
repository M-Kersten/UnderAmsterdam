using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandTeamwork : MonoBehaviour
{
    [SerializeField] private PlayerData myData;
    [SerializeField] private GameObject particle;
    [SerializeField] private Transform myCap;

    private void OnTriggerEnter(Collider other){
        if (other.gameObject.layer == 14 && other.gameObject.GetComponent<HandTeamwork>().myData.company != myData.company && this.transform.position.y > myCap.position.y){ // Teamwork layer
            if (TeamworkManager.Instance.AddTeamWork(myData.company, other.gameObject.GetComponent<HandTeamwork>().myData.company)) {
                GameObject pObj = Instantiate(particle, transform.position, Quaternion.identity);
                Destroy(pObj, pObj.GetComponent<ParticleSystem>().main.duration + 5f);
            }
        }
    }
}
