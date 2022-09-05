using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LTPathRequestManager : MonoBehaviour
{
    private Queue<PathRequest> m_pathRequestQueue = new Queue<PathRequest>();
    private PathRequest m_currentPathRequest;
    
    private LTPathFinding m_pathFinding;
    private bool m_isProcessingPath;
    public static LTPathRequestManager Instance;
    
    public void InitiatePathFindingManager(LTPathFinding pathfinding)
    {
        m_isProcessingPath = false;
        m_pathFinding = pathfinding;
        Instance = this;
    }

    public static void RequestPath(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(start, end, callback);
        Instance.m_pathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!m_isProcessingPath && m_pathRequestQueue.Count > 0)
        {
            m_currentPathRequest = m_pathRequestQueue.Dequeue();
            m_isProcessingPath = true;
            m_pathFinding.StartFindPath(m_currentPathRequest.start, m_currentPathRequest.end);
        }
    }

    public void FinishProcessPath(Vector3[] path, bool success)
    {
        m_currentPathRequest.callback(path, success);
        m_isProcessingPath = false;
        TryProcessNext();
    }
}

struct PathRequest
{
    public Vector3 start;
    public Vector3 end;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
    {
        this.start = start;
        this.end = end;
        this.callback = callback;
    }
}


