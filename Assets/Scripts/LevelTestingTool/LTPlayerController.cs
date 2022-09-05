using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class LTPlayerController : MonoBehaviour
{
    public List<GameObject> goalsList = new List<GameObject>();
    
    private int m_goalCount = 0;
    private PlayerInventory m_playerInventory;
    private Vector3[] m_path;
    private int m_targetIndex;
    
    // Start is called before the first frame update
    void Start()
    {
        m_playerInventory = GetComponent<PlayerInventory>();
        StartCoroutine(DelayStart());
    }

    IEnumerator DelayStart()
    {
        GameObject endLoc = GameObject.Find("EndLocation");
        if (endLoc == null)
        {
            PathUnsuccessful("There is no end position located");
            yield break;
        }
        
        yield return new WaitForSeconds(0.5f);
        TakeStepTowardGoal();
    }

    private void TakeStepTowardGoal()
    {
        if (goalsList.Count > 0)
        {
            int lastGoal = goalsList.Count - 1;
            int secondLastGoal = goalsList.Count - 2;
            LTPathRequestManager.RequestPath(goalsList[secondLastGoal].transform.position, goalsList[lastGoal].transform.position, OnOriginalGoalChecks);
        }
        else
        {
            int lastGoal = goalsList.Count - 1;
            LTPathRequestManager.RequestPath(transform.position, goalsList[lastGoal].transform.position, OnOriginalGoalChecks);
        }

        if (m_goalCount < goalsList.Count)
        {
            Vector3 position = goalsList[m_goalCount].transform.position;
            Vector3 newPosition = new Vector3(position.x, position.y + 0.5f, position.z);
            LTPathRequestManager.RequestPath(transform.position, newPosition, OnGoalFound);
        }
    }

    private void OnGoalFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            m_path = newPath;
            
            // need to check if its door and if it is we contain.
            if (goalsList[m_goalCount].CompareTag("Door"))
            {
                var door = goalsList[m_goalCount].GetComponent<Door>();
                int id = door.id;
                
                // if it does contain the id then we need to increase the goal
                if (m_playerInventory.MatchThisID(id))
                {
                    StartCoroutine(FollowPath());
                }
                else
                {
                    PathUnsuccessful("No " + goalsList[m_goalCount].name + " has been obtained that matches the current doors ID");
                }
            }
            else if (!goalsList[m_goalCount].CompareTag("Door"))
            {
                StartCoroutine(FollowPath());
            }
        }
        else
        {
            PathUnsuccessful("No clear pathfinding route to the target goal");
        }
    }

    private void OnOriginalGoalChecks(Vector3[] newPath, bool pathSuccessful)
    {
        if (!pathSuccessful)
        {
            PathUnsuccessful("No clear path to the end goal");
        }
    }
    
    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = m_path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                m_targetIndex++;
                if (m_targetIndex >= m_path.Length)
                {
                    m_goalCount++;
                    TakeStepTowardGoal();
                    yield break;
                }

                currentWaypoint = m_path[m_targetIndex];
            }

            Vector3 newWaypoint = new Vector3(currentWaypoint.x, currentWaypoint.y, currentWaypoint.z);
            transform.position = Vector3.MoveTowards(transform.position, newWaypoint, 2.0f * Time.deltaTime);
            yield return null;
        }
        
    }

    private void PathUnsuccessful(string message)
    {
        Debug.LogWarning("Path Failed : " + message);
    }
    
    public void OnDrawGizmos()
    {
        if (m_path != null)
        {
            for (int i = m_targetIndex; i < m_path.Length; i++)
            {
                if (i == m_targetIndex)
                {
                    Gizmos.DrawLine(transform.position, m_path[i]);
                }
                else
                {
                    Gizmos.DrawLine(m_path[i - 1], m_path[i]);
                }
            }
        }
    }
}
