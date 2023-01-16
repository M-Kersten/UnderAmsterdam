using UnityEngine;

public class RootGenerator : MonoBehaviour
{

    private GameObject rootGO;

    private void Start()
    {
        rootGO = new GameObject("Root");
        
        MeshFilter filter = rootGO.AddComponent<MeshFilter>();
        MeshRenderer renderer = rootGO.AddComponent<MeshRenderer>();
        
        Mesh mesh = new Mesh();
        
        var vertices = new Vector3[]
        {
            new Vector3(-0.5f, 0, 0), // front left leg
            new Vector3(0.5f, 0, 0), // front right leg
            new Vector3(-0.5f, 1, 0), // body
            new Vector3(0.5f, 1, 0), // body
            new Vector3(-0.5f, 2, 0), // back left leg
            new Vector3(0.5f, 2, 0), // back right leg
            new Vector3(0, 0.5f, 0) // head
        };
        mesh.vertices = vertices;
        
        int[] triangles = new int[]
        {
            0, 2, 1,
            2, 3, 1,
            2, 4, 3,
            4, 5, 3,
            2, 6, 4
        };
        
        mesh.triangles = triangles;
        filter.mesh = mesh;
    }


}