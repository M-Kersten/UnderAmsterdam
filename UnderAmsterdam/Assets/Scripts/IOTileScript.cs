using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class IOTileScript : NetworkBehaviour
{
    public int roundInputPipe;
    
    [SerializeField] private Color[] pipeColors;
    [SerializeField] private List<IOTileScript> IoNeighbourTiles;
    [SerializeField] private GameObject VisualObject;
    [SerializeField] private Renderer myRenderer;
    [SerializeField] private GameObject IndicatorPrefab;
    [SerializeField] private PipeParticles particles;
    [SerializeField] private float particlesBreathingTime;
    [SerializeField] private LayerMask pipeLayer;

    private ChangeDetector _changes;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    [Networked]
    public int Company { get; set; }

    [Networked] 
    public bool isOutput { get; set; }
    
    public override void Spawned()
    {
        _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
        GetNeighbours();
    }

    public override void Render()
    {
        foreach (var change in _changes.DetectChanges(this, out var previousBuffer, out var currentBuffer))
        {
            switch (change)
            {
                case nameof(Company):
                    var reader = GetPropertyReader<int>(nameof(Company));
                    var (previous,current) = reader.Read(previousBuffer, currentBuffer);
                    OnIOTileChanged(current);
                    break;
            }
        }
    }
    
    void OnIOTileChanged(int company)
    {
        if(Gamemanager.Instance.ConnectionManager.runner.IsClient)
            TryEnableIOPipe(company, isOutput, true);
    }
    
    private void GetNeighbours()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.up, out hit))
            if(hit.transform.gameObject.TryGetComponent(out IOTileScript tile))
                IoNeighbourTiles.Add(tile);

        if (Physics.Raycast(transform.position, Vector3.down, out hit))
            if (hit.transform.gameObject.TryGetComponent(out IOTileScript tile))
                IoNeighbourTiles.Add(tile);

        if (Physics.Raycast(transform.position, Vector3.left, out hit))
            if (hit.transform.gameObject.TryGetComponent(out IOTileScript tile))
                IoNeighbourTiles.Add(tile);

        if (Physics.Raycast(transform.position, Vector3.right, out hit))
            if (hit.transform.gameObject.TryGetComponent(out IOTileScript tile))
                IoNeighbourTiles.Add(tile);

        if (Physics.Raycast(transform.position, Vector3.forward, out hit))
            if (hit.transform.gameObject.TryGetComponent(out IOTileScript tile))
                IoNeighbourTiles.Add(tile);

        if (Physics.Raycast(transform.position, Vector3.back, out hit))
            if (hit.transform.gameObject.TryGetComponent(out IOTileScript tile))
                IoNeighbourTiles.Add(tile);
    }
    private bool CheckNeighboursOccupied()
    {
        for (int i = 0; i < IoNeighbourTiles.Count; i++)
            if (IoNeighbourTiles[i].Company != -1) 
                return true;

        return false;
    }
    public bool TryEnableIOPipe(int setCompany, bool shouldBeOutput, bool isSyncing)
    {
        if (setCompany == -1)
        {
            Company = setCompany;
            return false;
        }
        
        if (!isSyncing && Company != -1) 
            return false; // This IOTile already has got a company

        if (CheckNeighboursOccupied())
            return false;

        Company = setCompany;
        isOutput = shouldBeOutput;
        
        myRenderer.material.SetColor(BaseColor, pipeColors[Company]);
        VisualObject.SetActive(true);

        if (isOutput)
        {
            Gamemanager.Instance.RoundStart.AddListener(() => SpawnIndicator(true));

            //If we are on the first round then just spawn an indicator, round start is already ongoing.
            if (Gamemanager.Instance.currentRound == 1) 
                SpawnIndicator(true);
        }
        else
        {
            roundInputPipe = Gamemanager.Instance.currentRound;
            SpawnIndicator(false);
        }

        return true;
    }
    
    public void StartPipeCheck()
    {
        if (isOutput)
        {
            // Getting the tile in front of it
            RaycastHit hit;
            Ray ray = new Ray(transform.position, -transform.right);
            
            if (Physics.Raycast(ray, out hit, 3, pipeLayer))
            {
                // Launching the checking process
                CubeInteraction tile = hit.transform.GetComponent<CubeInteraction>();
                if(tile.Company != -1)
                    tile.CheckConnectionForWin();
            }
        }
    }

    public void SpawnIndicator(bool shouldBeOutput)
    {
        if (Company == Gamemanager.Instance.networkData.Company)
        {
            var indicatorScript = Instantiate(IndicatorPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity).GetComponent<InOutIndicatorScript>();
            indicatorScript.InitializeIndicator(shouldBeOutput);
            
            var pipeParticles = Instantiate(particles, transform);
            pipeParticles.SetColors(pipeColors[Company]);
        }
    }
}
