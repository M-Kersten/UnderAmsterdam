using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeColouring : MonoBehaviour
{
    [Tooltip("Circle between connecting pipes")]
    [SerializeField] private GameObject connectorPiece;
    [Tooltip("Parent of where all pipes are stored")]
    [SerializeField] private GameObject pipeParent;

    [Tooltip("All pipe options")]
    [SerializeField] private GameObject[] pipeChildren;

    private int LODlevels = 4;

    public void UpdateRenderer(string pipeCompany, GameObject givenGO = null)
    {
        if (!givenGO)
        {
            for (int i = 0; i < pipeChildren.Length; i++)
            {
                // Go through the children and find the active one
                if (pipeChildren[i].activeSelf == true)
                {
                    GameObject selectedChild = pipeChildren[i];

                    // Give the company and the gameobject where the materials are on
                    ColourSystem.Instance.SetColour(selectedChild.transform.GetChild(0).transform.GetChild(1).gameObject, pipeCompany);
                    ColourSystem.Instance.SetColour(selectedChild.transform.GetChild(0).transform.GetChild(2).gameObject, pipeCompany);

                    ColourSystem.Instance.SetColour(selectedChild.transform.GetChild(2).transform.GetChild(0).gameObject, pipeCompany);
                    ColourSystem.Instance.SetColour(selectedChild.transform.GetChild(2).transform.GetChild(1).gameObject, pipeCompany);

                    ColourSystem.Instance.SetColour(selectedChild.transform.GetChild(3).transform.GetChild(0).gameObject, pipeCompany);
                    ColourSystem.Instance.SetColour(selectedChild.transform.GetChild(3).transform.GetChild(1).gameObject, pipeCompany);

                }
            }
        }
        else
        {
            // Is anything other than a pipe, like connector piece
            for (int j = 0; j < LODlevels; j++)
                ColourSystem.Instance.SetColour(givenGO.transform.GetChild(j).gameObject, pipeCompany);
            
        }
    }
}