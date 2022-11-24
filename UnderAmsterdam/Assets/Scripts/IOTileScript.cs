using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class IOTileScript : NetworkBehaviour
{
    [SerializeField] private Material[] pipeMaterials;
    [SerializeField] private GameObject VisualObject;
    [SerializeField] private Renderer myRenderer;
    [SerializeField] private GameObject IndicatorPrefab;

    [Networked(OnChanged = nameof(OnIOTileChanged))]
    public string company { get; set; }

    [Networked] public bool isOutput { get; set; }

    public override void Spawned()
    {
        company = "Empty"; //Set company to default
    }

    public bool TryEnableIOPipe(string setCompany, bool shouldBeOutput, bool isSyncing)
    {
        if (setCompany == "Empty")
        {
            company = setCompany;
            return false;
        }
        if (!isSyncing && company != "Empty") return false; // This IOTile already has got a company

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

        if (isOutput)
            Gamemanager.Instance.RoundStart.AddListener(delegate { SpawnIndicator(true); });

        SpawnIndicator(shouldBeOutput);
        return true;
    }

    static void OnIOTileChanged(Changed<IOTileScript> changed)
    {
        changed.Behaviour.TryEnableIOPipe(changed.Behaviour.company, changed.Behaviour.isOutput, true);
    }

    public void StartPipeCheck()
    {
        if (isOutput)
        {
            // Getting the tile in front of it
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.right, out hit))
            {
                // Launching the checking process
                CubeInteraction tile = hit.transform.GetComponent<CubeInteraction>();
            }
        }
    }

    public void SpawnIndicator(bool shouldBeOutput)
    {
        if (company == Gamemanager.Instance.localPlayerData.company)
        {
            InOutIndicatorScript indicatorScript = Instantiate(IndicatorPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity).GetComponent<InOutIndicatorScript>();
            indicatorScript.InitializeIndicator(shouldBeOutput);
        }
    }
}
