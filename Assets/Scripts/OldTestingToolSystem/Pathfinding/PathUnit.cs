using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PathUnit : MonoBehaviour
{
    private DefaultInput input;
    private int stepCount = 0;
    public List<Transform> transformSteps = new List<Transform>();
    public Transform target;

    private float speed = 5.0f;
    private Vector3[] path;
    private int targetIndex;

    private GameObject[] keyArray;
    private GameObject[] doorArray;
    private GameObject startPosition;
    private GameObject endPosition;

    private bool keyFound;
    private bool doorFound;

    private int keyIndex;
    private int doorIndex;

    private void Awake()
    {
        input = new DefaultInput();
        input.Player.Jump.performed += e => TakeStep();
        input.Enable();
        
        keyArray = GameObject.FindGameObjectsWithTag("Key");
        doorArray = GameObject.FindGameObjectsWithTag("Door");
        startPosition = GameObject.FindWithTag("StartVolume");
        endPosition = GameObject.FindWithTag("EndVolume");

        // need to save conditions for each key,
        // save location for each of the doors
        // save location for end position

        // should probably do this in a level completion level editor?

        // move some of the functionality to the level completion level editor?
    }

    // Start is called before the first frame update
    void Start()
    {
        // calculate the order of steps
        // check each key, if a key location is found, search for door, if no door then search for another key?
        for (int i = 0; i <= keyArray.Length -1; i++)
        {
            //PathRequestManager.RequestPath(transform.position, keyArray[i].transform.position, OnKeyPathFound);
            keyIndex = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CalculateSteps()
    {
        
    }

    private void OnKeyPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            //key found, add it to the steps
            transformSteps.Add(keyArray[keyIndex].transform);
            
            //remove this key from the array?
            
            // find a corresponding door in the level that you can move to that key ID
        }
    }

    private void OnDoorPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            transformSteps.Add(doorArray[doorIndex].transform);
            //disable this door?
        }
        else
        {
            Debug.Log("Door not found");
        }
    }

    private void OnEndLocationFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            
        }
    }
    
    void TakeStep()
    {
        if (stepCount < transformSteps.Count)
        {
            PathRequestManager.RequestPath(transform.position, keyArray[stepCount].transform.position, OnKeyPathFound);
            stepCount++;
        }
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

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            Vector3 newWaypoint = new Vector3(currentWaypoint.x, currentWaypoint.y, currentWaypoint.z);
            transform.position = Vector3.MoveTowards(transform.position, newWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }
    
    public void OnDrawGizmos() {
        if (path != null) {
            for (int i = targetIndex; i < path.Length; i ++) {
                if (i == targetIndex) {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else {
                    Gizmos.DrawLine(path[i-1],path[i]);
                }
            }
        }
    }
}
