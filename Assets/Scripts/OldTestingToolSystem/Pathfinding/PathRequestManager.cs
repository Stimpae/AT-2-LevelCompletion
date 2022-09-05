using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest currentPathRequest;

    private static PathRequestManager instance;
    private Pathfinding _pathfinding;
    private PathfindingGrid _grid;

    private bool isProcessingPath;
    
    public void InitPathFindingManager()
    {
        instance = this;
        isProcessingPath = false;
        _pathfinding = GetComponent<Pathfinding>();
        _grid = GetComponent<PathfindingGrid>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance._grid.CreateGrid();
        instance.TryProcessNext();
    }

    void TryProcessNext()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            _pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
