using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LTGrid
{
    [SerializeField]private int m_width;
    [SerializeField] private int m_height;
    [SerializeField]private float m_nodeSize;
    [SerializeField]public LTNode[] gridArray;

    public int GetWidth()
    {
        return m_width;
    }
    
    public LTGrid(int width, int height, float nodeSize, bool drawDebug)
    {
        // assign values to private vars
        m_width = width;
        m_height = height;
        m_nodeSize = nodeSize;
        gridArray = new LTNode[width * height];

        // loop through each axis of the grid 
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridArray[x * width + z] = new LTNode(x, z);
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x,0, z) * m_nodeSize;
    }

    public Vector2Int GetXZPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / m_nodeSize);
        int z = Mathf.FloorToInt(worldPosition.z / m_nodeSize);
        return new Vector2Int(x,z);
    }
    
    public LTNode GetNodeAtWorldPosition(Vector2 position)
    {
        int x = Mathf.FloorToInt(position.x);
        int z = Mathf.FloorToInt(position.y);
        return gridArray[x * m_width + z];
    }

    public LTNode GetNode(int x, int z)
    {
        return gridArray[x * m_width + z];
    }
    
}
