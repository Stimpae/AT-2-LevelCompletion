using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

[ExecuteInEditMode]
public class EditorCompletionHelper : MonoBehaviour
{
    public GameObject player;
    public Transform startPosition;

    [NonSerialized] public bool success;
    [NonSerialized] public bool failure;
    [NonSerialized] public string suggestions;
    [NonSerialized] public string newSuggestions1;
    [NonSerialized] public string newSuggestions2;

    private Vector3[] path;
    private int targetIndex;
    public List<int> _keyList = new List<int>();

    private PathRequestManager _requestManager;
    private Pathfinding _pathfinding;
    private PathfindingGrid _grid;

    public void Reset()
    {
        _keyList = new List<int>();
        success = false;
        failure = false;
        suggestions = "";
        newSuggestions1 = "";
        newSuggestions2 = "";
        player.transform.position = startPosition.position;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            Debug.Log("Path successful");
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
        else
        {
            Debug.Log("Path not successful - Attempt next step");
        }
    }

    public void OnDoorPathFound(Vector3[] newPath, bool pathSuccessful)
    {
    }

    public void OnEndPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            success = true;
        }
        else
        {
            suggestions = "the end goal cannot be reached in its current position";
            newSuggestions1 = " - add a door to the end goal.";
            newSuggestions2 = " - move the end goal to a reachable position.";
            failure = true;
        }
    }

    public IEnumerator AttemptSteps(List<GameObject> steps)
    {
        _keyList = new List<int>();
        _requestManager = GetComponent<PathRequestManager>();
        _requestManager.InitPathFindingManager();

        _pathfinding = GetComponent<Pathfinding>();
        _pathfinding.InitPathfinding();

        _grid = GetComponent<PathfindingGrid>();

        if (!failure)
        {
            foreach (var go in steps.ToArray())
            {
                switch (go.tag)
                {
                    case "Key":
                    {
                        _keyList.Add(go.GetComponent<Key>().id);
                        go.GetComponent<Key>().gameObject.SetActive(false);
                        break;
                    }
                    case "Door":
                    {
                        foreach (var key in _keyList)
                        {
                            if (go.transform.parent.GetComponent<Door>().id == key)
                            {
                                go.transform.parent.gameObject.SetActive(false);
                            }
                            else
                            {
                                failure = true;
                                suggestions = "you are holding no key assigned to door - " +
                                              go.transform.parent.GetComponent<Door>().id;
                                newSuggestions1 = " - change the door id to match an appropriate key in the level.";
                                newSuggestions2 = " - add a new key in the level to match this door id.";
                                break;
                            }
                        }

                        break;
                    }
                    case "EndVolume":
                    {
                        PathRequestManager.RequestPath(player.transform.position, go.transform.position,
                            OnEndPathFound);
                    }
                        break;
                }
                PathRequestManager.RequestPath(player.transform.position, go.transform.position, OnPathFound);
                yield return new EditorWaitForSeconds(2f);
            }
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (player.transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            Vector3 newWaypoint = new Vector3(currentWaypoint.x, currentWaypoint.y, currentWaypoint.z);
            player.transform.position = newWaypoint;
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    public void Update()
    {
        
    }
}