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
    public void SetColour(GameObject givenGO, string companyName)
    {
        if (givenGO.layer == 8)//hand layer
        {
            for (int i = 0; i < handMaterials.Length; i++)
                if (companyName + "Hand" == handMaterials[i].name)
                    givenGO.GetComponent<Renderer>().material = handMaterials[i];
        }
        else if(givenGO.tag == "misc")
        {
            for (int i = 0; i < miscMaterials.Length; i++)
                if (companyName + "Misc" == miscMaterials[i].name)
                    givenGO.GetComponent<Renderer>().material = miscMaterials[i];
        }
        else
        {
            for (int i = 0; i < pipeMaterials.Length; i++)
                if (companyName == pipeMaterials[i].name)
                    givenGO.GetComponent<Renderer>().material = pipeMaterials[i];
        }
    }
}
