using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class LTPathFinding : MonoBehaviour
{
    [SerializeField] public LTGrid m_grid;
    [SerializeField] public static LTPathFinding Instance { get; private set; }
    
    [SerializeField] private List<LTNode> openList;
    [SerializeField] private List<LTNode> closedList;

    [SerializeField] public LTPathRequestManager m_requestManager;

    public void InitialisePathFinding(int width, int height, LTPathRequestManager manager)
    {
        m_requestManager = manager;
        m_grid = new LTGrid(width, height, 1, true);
        Instance = this;
    }
    
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        
        StartCoroutine(FindPath((int)startPos.x, (int)startPos.z, (int)targetPos.x, (int)targetPos.z));
    }
    
    IEnumerator FindPath(int startX, int startZ, int endX, int endY)
    {
        LTNode startingNode = m_grid.GetNode(startX, startZ);
        LTNode endNode = m_grid.GetNode(endX, endY);
        
       
        // start with the start node in open list as its always where we are starting
        
        openList = new List<LTNode> {startingNode};
        closedList = new List<LTNode>();
        
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        for (int x = 0; x < m_grid.GetWidth(); x++)
        {
            for (int z = 0; z < m_grid.GetWidth(); z++)
            {
                // initialise all of our nodes currently in the grid
                LTNode pathNode = m_grid.GetNodeAtWorldPosition(new Vector2(x, z));
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.previousNode = null;
            }
        }

        startingNode.gCost = 0;
        startingNode.hCost = GetDistance(startingNode, endNode);
        startingNode.CalculateFCost();

        while (openList.Count > 0)
        {
            LTNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                // we have reached our final node
                pathSuccess = true;
                break;
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbour in GetNeighbours(currentNode))
            {
                if(closedList.Contains(neighbour)) continue;
                if (!neighbour.isWalkable)
                {
                    closedList.Add(neighbour);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (tentativeGCost < neighbour.gCost)
                {
                    neighbour.previousNode = currentNode;
                    neighbour.gCost = tentativeGCost;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.CalculateFCost();

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }

        yield return null;
        
        // Completely out of nodes on the open list // no path could be found
        if (pathSuccess)
        {
            waypoints = RetracePath(startingNode, endNode);
        }
        
        m_requestManager.FinishProcessPath(waypoints, pathSuccess);
    }

    private List<LTNode> GetNeighbours(LTNode node)
    {
        List<LTNode> neighbours = new List<LTNode>();
        
        // left
        if (node.x - 1 >= 0)
        {
            neighbours.Add(m_grid.GetNodeAtWorldPosition(new Vector3(node.x - 1, node.z)));
            
            if(node.z - 1 >= 0) neighbours.Add(m_grid.GetNodeAtWorldPosition(new Vector3(node.x - 1, node.z - 1)));
            if(node.z + 1 < m_grid.GetWidth()) neighbours.Add(m_grid.GetNodeAtWorldPosition(new Vector3(node.x - 1, node.z + 1)));
        }
        
        // right
        if (node.x + 1 < m_grid.GetWidth())
        {
            neighbours.Add(m_grid.GetNodeAtWorldPosition(new Vector3(node.x + 1, node.z)));
            
            if(node.z - 1 >= 0) neighbours.Add(m_grid.GetNodeAtWorldPosition(new Vector3(node.x +  1, node.z - 1)));
            if(node.z + 1 < m_grid.GetWidth()) neighbours.Add(m_grid.GetNodeAtWorldPosition(new Vector3(node.x+ 1, node.z + 1)));
        }
        
        // down
        if (node.z - 1 >= 0)
        {
            neighbours.Add(m_grid.GetNodeAtWorldPosition(new Vector3(node.x, node.z - 1)));
        }
        
        // up
        if (node.z + 1 >= m_grid.GetWidth())
        {
            neighbours.Add(m_grid.GetNodeAtWorldPosition(new Vector3(node.x, node.z + 1)));
        }
        
        return neighbours;
    }
    
    Vector3[] RetracePath(LTNode startNode, LTNode endNode) {
        List<LTNode> path = new List<LTNode>();
        LTNode currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.previousNode;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<LTNode> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i-1].x - 
                                               path[i].x,path[i-1].z - path[i].z);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i-1].worldPosition);
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    private LTNode GetLowestFCostNode(List<LTNode> list)
    {
        LTNode lowestFCostNode = list[0];
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = list[i];
            }
        }
        return lowestFCostNode;
    }
    
    private int GetDistance(LTNode nodeA, LTNode nodeB)
    {
        int Ax = Mathf.FloorToInt(nodeA.worldPosition.x);
        int Bx = Mathf.FloorToInt(nodeB.worldPosition.x);
        
        int Az = Mathf.FloorToInt(nodeA.worldPosition.z);
        int Bz = Mathf.FloorToInt(nodeB.worldPosition.z);
        
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.z - nodeB.z);

        if (dstX > dstY)
            return 14*dstY + 10* (dstX-dstY);
        return 14*dstX + 10 * (dstY-dstX);
    }

}
