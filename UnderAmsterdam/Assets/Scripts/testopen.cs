using UnityEngine;

public class testopen : MonoBehaviour
{
        void Start()
    {
// Create a new empty game object
        GameObject cat = new GameObject("Cat");

// Add a MeshFilter and MeshRenderer component to the game object
        MeshFilter filter = cat.AddComponent<MeshFilter>();
        MeshRenderer renderer = cat.AddComponent<MeshRenderer>();

// Create a new mesh for the cat
        Mesh mesh = new Mesh();

// Set the vertices of the mesh
        Vector3[] vertices = new Vector3[] {
            new Vector3(-0.5f, 0, 0),  // front left leg
            new Vector3(0.5f, 0, 0),   // front right leg
            new Vector3(-0.5f, 1, 0),  // body
            new Vector3(0.5f, 1, 0),   // body
            new Vector3(-0.5f, 2, 0),  // back left leg
            new Vector3(0.5f, 2, 0),   // back right leg
            new Vector3(0, 0.5f, 0),   // head
        };
        mesh.vertices = vertices;

// Set the triangles of the mesh
        int[] triangles = new int[] {
            0, 2, 1,  // front left leg
            2, 3, 1,  // front body
            2, 4, 3,  // back body
            4, 5, 3,  // back right leg
            2, 6, 4   // head
        };
        mesh.triangles = triangles;

// Set the mesh on the filter
        filter.mesh = mesh;
        }


}