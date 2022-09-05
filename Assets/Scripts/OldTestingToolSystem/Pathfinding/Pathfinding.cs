using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class Pathfinding : MonoBehaviour
{

    private PathRequestManager _requestManager;
    public PathfindingGrid grid;

    public void InitPathfinding()
    {
        _requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<PathfindingGrid>();
    }
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        EditorCoroutineUtility.StartCoroutine(FindPath(startPos, targetPos), this);
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
        Node startNode = grid.NodeFromWorldPosition(startPos);
        Node targetNode = grid.NodeFromWorldPosition(targetPos);
        
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node node = openSet.RemoveFirst();
                closedSet.Add(node);

                if (node == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(node))
                {
                    if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = node;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            yield return null;
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
            }
            
            _requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i-1].nodePosition);
            }
            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14*dstY + 10* (dstX-dstY);
        return 14*dstX + 10 * (dstY-dstX);
    }
}