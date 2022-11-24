using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOTileScript : MonoBehaviour
{
    [SerializeField] private Material[] pipeMaterials;
    [SerializeField] private GameObject VisualObject;
    [SerializeField] private Renderer myRenderer;
    [SerializeField] private GameObject IndicatorPrefab;

    public bool isOutput;
    public string company;

    private void Start()
    {
        company = "Empty"; //Set company to default
    }
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
            if (company == Gamemanager.Instance.localPlayerData.company) {
                InOutIndicatorScript indicatorScript = Instantiate(IndicatorPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity).GetComponent<InOutIndicatorScript>();
                indicatorScript.InitializeIndicator(shouldBeOutput);
            }
            return true;
        }
        else return false;
    }

    public void StartPipeCheck()
    {
        if (isOutput)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.right, out hit))
            {
                CubeInteraction tile = hit.transform.GetComponent<CubeInteraction>();
                if(tile.company != "Empty")
                    tile.CheckConnectionForWin();
            }
        }
    }
}
