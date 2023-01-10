using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RootGrower : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> rootMeshRenderer;
    private List<Material> rootMat = new List<Material>();
    
    [SerializeField] private float maxGrow = 0.876f;
    
    [Tooltip("Time to fully grow in seconds")]
    [SerializeField] float timeToGrow = 4f;
    
    [SerializeField] private float refreshRate;
    private static readonly int GrowSteps = Shader.PropertyToID("_GrowSteps");

    private int amountOfRound;
    private int currentRound;

    [SerializeField] private BoxCollider[] rootCollider = new BoxCollider[6];
    private MeshCollider rootColliderMesh;

    void Start()
    {
        for (int i = 0; i < rootMeshRenderer.Count; ++i)
        {
            for (int j = 0; j < rootMeshRenderer[i].materials.Length; ++j)
            {
                if (rootMeshRenderer[i].materials[j].HasProperty(GrowSteps))
                {
                    rootMeshRenderer[i].materials[j].SetFloat(GrowSteps, 0f);
                    rootMat.Add(rootMeshRenderer[i].materials[j]);
                }
            }
        }

        amountOfRound = Gamemanager.Instance.amountOfRounds;
        
        //RandManager.Instance.randomGrowOn.AddListener();
    }
    
    void Update()
    {
        currentRound = Gamemanager.Instance.currentRound;
        
        
        
        for (int i = 0; i < rootMat.Count; i++)
        {
            StartCoroutine(Grower(rootMat[i]));
        }
        
    }
    
    IEnumerator Grower(Material mat)
    {
        float growSteps = mat.GetFloat(GrowSteps);
        float growsRoundVal = (float) currentRound / (float) amountOfRound;


        //Collider activation
        for (int i = 0; i < rootCollider.Length; i++)
        {
            if(growSteps >= (float) (i + 0.5f) / (float)rootCollider.Length)
                rootCollider[i].enabled = true;
        }
        
        growsRoundVal *= Random.Range(0.9f, 1.1f);
        
        while (growSteps < maxGrow && growSteps < growsRoundVal)
        {
            growSteps += 1 / (timeToGrow / refreshRate);
                
            mat.SetFloat(GrowSteps, growSteps);
            yield return new WaitForSeconds (refreshRate);
        }


    }
}

    