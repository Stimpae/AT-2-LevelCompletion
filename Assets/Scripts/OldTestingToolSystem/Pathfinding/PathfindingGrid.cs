using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]  
public class PathfindingGrid : MonoBehaviour
{
    public bool displayGridDebug;
    public Vector2 gridSize;
    public float nodeRadius = 1f;
    public LayerMask unwalkableMask;
    public Mesh debugMesh;
    
    public Transform playerStart;
    public Transform endTransform;
    
    // this could probably be an array or list that we iterate through? assign positions to the
    // pathfinding grid.
    public Transform keyTransform;
    private Node[,] grid;

    private float nodeDiameter;
    private int gridSizeX;
    private int gridSizeY;
    
    private void OnDrawGizmos()
    { 
        if (grid != null && displayGridDebug)
        {
            // this needs to happen automatically
            Node playerNode = NodeFromWorldPosition(playerStart.position);
            Node endNode = NodeFromWorldPosition(endTransform.position);
            Node keyNode = NodeFromWorldPosition(keyTransform.position);
            
            foreach (var node in grid)
            {
                Gizmos.color = Color.gray;
                Gizmos.color = (node.isWalkable) ? Color.white : Color.red;
                if (playerNode == node)
                {
                    Gizmos.color = Color.green;
                }

                if (endNode == node)
                {
                    Gizmos.color = Color.yellow;
                }

                if (keyNode == node)
                {
                    Gizmos.color = Color.blue;
                }
                
                Gizmos.DrawMesh(debugMesh, node.nodePosition, Quaternion.identity, new Vector3(0.1f,0.1f,0.1f));
            }
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, 1, gridSize.y));
        }
    }

    // Start is called before the first frame update
    [ContextMenu("Load Node Map")]
    void Awake()
    {
        
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPosition.z + gridSize.y / 2) / gridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for(int x = -1; x <=1; x++)
        {
            for(int y = -1; y <=1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX,checkY]);
                }
            }
        }

        return neighbours;
    }

    public void CreateGrid()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / nodeDiameter);

        // create a new grid;
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSize.x / 2 - Vector3.forward * gridSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius)
                                                     + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius - 0.1f, unwalkableMask));
                grid[x,y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
