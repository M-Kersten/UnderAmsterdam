using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeColouring : MonoBehaviour
{
    [SerializeField] Material[] pipeMaterials;
    private GameObject childParent;
    private GameObject activeChild;

    public void UpdateRenderer(string pipeCompany) {

        childParent = transform.GetChild(0).transform.GetChild(0).gameObject;

        for (int i = 0; i < childParent.transform.childCount; i++)
        {
            if(childParent.transform.GetChild(i).gameObject.activeSelf == true)
            {
                activeChild = childParent.transform.GetChild(i).transform.GetChild(0).gameObject;
                TestColour(pipeCompany, activeChild);
            }
        }
    }

    void TestColour(string company, GameObject affectedChild) {
        
        for (int i = 0; i < pipeMaterials.Length; i++)
        {
            if (pipeMaterials[i].name == company) {
                affectedChild.GetComponent<Renderer>().material = pipeMaterials[i];
            }
        }
    }

    //void SetColour(string company) {
    //    switch (company) {
    //        case "data":
    //             activeChild.GetComponent<Renderer>().material = newMaterialRef;
    //        break;
    //        case "water":
    //             activeChild.GetComponent<Renderer>().material = newMaterialRef;
    //        break;
    //        case "sewage":
    //             activeChild.GetComponent<Renderer>().material = newMaterialRef;
    //        break;
    //        case "gas":
    //             activeChild.GetComponent<Renderer>().material = newMaterialRef;
    //        break;
    //        case "power":
    //             activeChild.GetComponent<Renderer>().material = newMaterialRef;
    //        break;
    //        default: 
    //
    //        break;
    //    }
    //}
}
