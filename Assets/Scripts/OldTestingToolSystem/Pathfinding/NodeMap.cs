using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMap : MonoBehaviour
{
    [Header("Node Map Size")] 
    public int width;
    public int height;
    public int nodeSize;

    [Header("References")] 
    public Mesh meshRef;

    private Node[,] mapArray;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [ContextMenu("Load Node Map")]
    void LoadNodeMap()
    {
        mapArray = new Node[width,height];
        for (int x = 0; x < mapArray.GetLength(0); x++)
        {
            for (int y = 0; y < mapArray.GetLength(1); y++)
            {
                //mapArray[x,y] = new Node(GetWorldPosition(x,y));
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x,y+1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x,y), GetWorldPosition(x + 1,y), Color.white, 100f);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (mapArray != null)
        {
            foreach (var node in mapArray)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawMesh(meshRef, 
                    new Vector3(node.nodePosition.x + 0.5f, -0.1f, node.nodePosition.z + 0.5f),
                    Quaternion.identity, new Vector3(0.1f,0,0.1f));
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0,y) * nodeSize;
    }
}
