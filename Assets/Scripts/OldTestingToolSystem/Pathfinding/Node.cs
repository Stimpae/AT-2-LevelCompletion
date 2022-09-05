using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool isWalkable;
    public Vector3 nodePosition;
    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;

    public Node parent;
    private int heapIndex;

    public Node(bool isWalkable, Vector3 pos, int gridX, int gridY)
    {
        this.isWalkable = isWalkable;
        nodePosition = pos;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}