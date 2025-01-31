using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class Circle2D : ScriptableObject
{
    [Range(0.005f,2)]
    [SerializeField] float baseRadius = 0.32f;
    
    [Range(0,1)]
    [SerializeField] float endRadius = 0.12f;
    
    [Range(0,512)]
    public int angularCounts = 16;

    [System.Serializable]
    public class Vertex
    {
        public Vector2 point;
        public Vector2 normal;
        public float uv;
    }
    
    public List<Vertex> vertices;
    public int lineCount => vertices.Count;

    private const float TAU = 6.283185307179586f;
    
    public void UpdateVertices(float t)
    {
        vertices.Clear();
        float radius = Mathf.LerpAngle(baseRadius, endRadius, t);
        for (int i = 0; i < angularCounts; i++)
        {
            float angle = ((float) i / (float) angularCounts) * TAU;
            vertices.Add(new Vertex());
            vertices[i].point = new Vector2(Mathf.Cos(angle) * radius,
                                             Mathf.Sin(angle) * radius);
            vertices[i].uv = angle;
            vertices[i].normal = Vector3.forward;
        }
    }
}
