using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourSystem : MonoBehaviour
{
    [Tooltip("All pipe materials, named same as company")]
    [SerializeField] private Material[] pipeMaterials;
    [SerializeField] private Material[] handMaterials;
    [SerializeField] private Material[] miscMaterials;
    public static ColourSystem Instance;

    void Start()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(this);

    }

    // Get the colour for this player
    public Material GetColour(string company)
    {
        for (int i = 0; i < pipeMaterials.Length; i++)
        {
            if (pipeMaterials[i].name == company)
            {
                return pipeMaterials[i];
            }
        }
        return pipeMaterials[pipeMaterials.Length-1];
    }

    [Tooltip("GameObject has to have renderer on it")]
    public void SetColour(GameObject givenGO, int companyName)
    {
        if (givenGO.layer == 8)//hand layer
        {
            for (int i = 0; i < handMaterials.Length; i++)
                if (Enum.GetValues(typeof(CompanyType)).GetValue(companyName) + "Hand" == handMaterials[i].name)
                    givenGO.GetComponent<Renderer>().material = handMaterials[i];
        }
        else if(givenGO.CompareTag("misc"))
        {
            for (int i = 0; i < miscMaterials.Length; i++)
                if (Enum.GetValues(typeof(CompanyType)).GetValue(companyName) + "Misc" == miscMaterials[i].name)
                    givenGO.GetComponent<Renderer>().material = miscMaterials[i];
        }
        else
        {
            if (companyName < 0)
                return;
            
            foreach (var pipe in pipeMaterials)
                if (Enum.GetValues(typeof(CompanyType)).GetValue(companyName).ToString() == pipe.name)
                    givenGO.GetComponent<Renderer>().material = pipe;
        }
    }
}
