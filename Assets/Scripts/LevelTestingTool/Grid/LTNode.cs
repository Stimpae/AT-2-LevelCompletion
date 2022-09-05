using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LTNode
{
    // being a bit lazy here
    [SerializeField] public int x;
    [SerializeField] public int z;
    [SerializeField] private GameObject go;

    [SerializeField] public int gCost;
    [SerializeField] public int hCost;
    [SerializeField] public int fCost;

    [SerializeField] public bool isWalkable = true;
    [SerializeField] public Vector3 worldPosition;
    [SerializeField] public LTNode previousNode;

    public LTNode(int x, int z)
    {
        this.x = x;
        this.z = z;
        worldPosition = new Vector3(x, 0, z);
        isWalkable = true;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public GameObject DebugNodeMesh(GameObject parent)
    {
        go = new GameObject();
        go.name = "DebugTile";
        go.tag = "Debug";
        go.transform.SetParent(parent.transform);
        
        Mesh newMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        
        vertices.Add(new Vector3(0,0,0));
        vertices.Add(new Vector3(1,0,0));
        vertices.Add(new Vector3(0,0,1));
        vertices.Add(new Vector3(1,0,1));
        newMesh.vertices = vertices.ToArray();
        
        uvs.Add(new Vector2(0,0));
        uvs.Add(new Vector2(1,0));
        uvs.Add(new Vector2(0,1));
        uvs.Add(new Vector2(1,1));
        newMesh.uv = uvs.ToArray();
        
        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(3);
        triangles.Add(1);
        newMesh.triangles = triangles.ToArray();
        newMesh.RecalculateNormals();

        go.AddComponent<MeshFilter>().sharedMesh = newMesh;
        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshCollider>().sharedMesh = newMesh;
        
        return go;
    }


}
