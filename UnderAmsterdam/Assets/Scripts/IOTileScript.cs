using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOTileScript : MonoBehaviour
{
    [SerializeField] private Material[] pipeMaterials;
    [SerializeField] GameObject VisualObject;
    [SerializeField] private Renderer myRenderer;

    public bool isOutput;
    public string company;

    public bool TryEnableIOPipe(string setCompany, bool shouldBeOutput)
    {
        if (company == "Empty")
        {
            company = setCompany;
            isOutput = shouldBeOutput;
            for (int i = 0; i < pipeMaterials.Length; i++)
            {
                if (pipeMaterials[i].name == company)
                {
                    myRenderer.material = pipeMaterials[i];
                }
            }
            VisualObject.SetActive(true);
            return true;
        }
        else return false;
    }
}
