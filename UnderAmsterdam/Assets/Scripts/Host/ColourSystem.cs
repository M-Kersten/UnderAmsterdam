using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourSystem : MonoBehaviour
{
    [Tooltip("All pipe materials, named same as company")]
    [SerializeField] private Material[] pipeMaterials;
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
    public void SetColour(GameObject givenGO, string company)
    {
        for (int i = 0; i < pipeMaterials.Length; i++)
        {
            if (pipeMaterials[i].name == company)
            {
                givenGO.GetComponent<Renderer>().material = pipeMaterials[i];
            }
        }
    }
}
