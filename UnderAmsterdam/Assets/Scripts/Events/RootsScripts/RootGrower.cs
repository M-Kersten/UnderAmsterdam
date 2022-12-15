using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RootGrower : MonoBehaviour
{
    [SerializeField] private List<MeshRenderer> rootMeshRenderer;
    private List<Material> rootMat = new List<Material>();
    
    [SerializeField] private float maxGrow = 0.876f;
    
    [Tooltip("Time to fully grow in seconds")]
    [SerializeField] float timeToGrow = 4f;
    
    [SerializeField] private float refreshRate;
    private static readonly int GrowSteps = Shader.PropertyToID("_GrowSteps");

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
    }
    
    void Update()
    {
        for (int i = 0; i < rootMat.Count; i++)
        {
            StartCoroutine(Grower(rootMat[i]));
        }

        IEnumerator Grower(Material mat)
        {
            float growSteps = mat.GetFloat(GrowSteps);
    
            while (growSteps < maxGrow)
            {
                growSteps += 1 / (timeToGrow / refreshRate);
                
                mat.SetFloat(GrowSteps, growSteps);
                yield return new WaitForSeconds (refreshRate);
            }

        }
    }
}

    