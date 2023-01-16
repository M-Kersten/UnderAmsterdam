using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RootSegment : MonoBehaviour
{

    [SerializeField] private Transform[] controlPoints = new Transform[4];
    [SerializeField] private Transform[] colliderTransform = new Transform[6];
    [SerializeField] private BezierPoint testPoint;
    

    [Range(0, 1)]
    [SerializeField] private float t = 0;
    [Range(0, 1024)]
    [SerializeField] private int circleCount = 0;
    [SerializeField] private Circle2D circle2D;
    
    private Mesh mesh;

    private Vector3 GetPos(ushort i) => controlPoints[i].position;

    private void Update() => GenerateMesh();

    private void Awake()
    {
        mesh = new Mesh { name = "root" };
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void GenerateMesh()
    {
        mesh.Clear();
        
        //Vertices uvs and normals
        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        
        for (int circleNum = 0; circleNum < circleCount; circleNum++)
        {
            float t = (float) circleNum / (float)(circleCount - 1);
            BezierPoint currentPoint = GetBezierPoint(t);
            circle2D.UpdateVertices(t);
            
            for (int edgeNum = 0; edgeNum < circle2D.vertices.Count; edgeNum++)
            {
                verts.Add(currentPoint.LocalToWorld((Vector3) circle2D.vertices[edgeNum].point));
                normals.Add(currentPoint.LocalToWorldVec((Vector3) circle2D.vertices[edgeNum].point));
                uvs.Add(new Vector2(circle2D.vertices[edgeNum].uv , t));
            }
        }
        
        //Triangle
        List<int> triangle = new List<int>();
        
        for (int circleNum = 0; circleNum < circleCount - 1; circleNum++)
        {
            int startIndex = circleNum * circle2D.angularCounts;
            int startIndexNext = (circleNum + 1)  * circle2D.angularCounts;
            
            for (int line = 0; line < circle2D.lineCount; line++)
            {
                int currentA = startIndex  + line;
                int currentB = startIndex  + (line + 1) % circle2D.lineCount;
                int nextA    = startIndexNext + line;
                int nextB    = startIndexNext + (line + 1) % circle2D.lineCount;
                
                triangle.Add(nextA);
                triangle.Add(currentA);
                triangle.Add(nextB);
                
                triangle.Add(nextB);
                triangle.Add(currentA);
                triangle.Add(currentB);
                
            }
        }

        mesh.SetVertices(verts);
        mesh.SetTriangles(triangle, 0);
        mesh.SetUVs(0 ,uvs);
        //mesh.RecalculateNormals();
        mesh.SetNormals(normals);

    }
    
    public void OnDrawGizmos()
    {
        for (ushort i = 0; i < controlPoints.Length; i++)
            Gizmos.DrawSphere(controlPoints[i].position, 0.1f);

        Handles.DrawBezier
        (
            GetPos(0),
            GetPos(3),
            GetPos(1),
            GetPos(2),
            Color.gray, Texture2D.grayTexture, 1f
        );

        testPoint = GetBezierPoint(t);

        void drawPoint(Vector3 pos)
        {
            Gizmos.DrawSphere(testPoint.LocalToWorld(pos), 0.002f);
        }

        circle2D.UpdateVertices(t);

        for (int i = 0; i < circle2D.vertices.Count; i++)
        {
            drawPoint((Vector3) circle2D.vertices[i].point);
        }
        
        //Handles.PositionHandle(testPoint.pos, testPoint.rot);
        
        //Colliders follows the curve
        for (int i = 0; i < colliderTransform.Length; i++)
        {
            BezierPoint bezierPoint = GetBezierPoint((float)(i + 0.5f) / colliderTransform.Length);
            colliderTransform[i].rotation = bezierPoint.rot;
            colliderTransform[i].position = bezierPoint.pos;
        }


    }

    private BezierPoint GetBezierPoint(float t)
    {
        Vector3 p0 = GetPos(0);
        Vector3 p1 = GetPos(1);
        Vector3 p2 = GetPos(2);
        Vector3 p3 = GetPos(3);

        //Lerps functions to find a point on the curve at t time and its direction
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);

        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        return new BezierPoint(Vector3.Lerp(d, e, t), (e - d).normalized);
    }
}