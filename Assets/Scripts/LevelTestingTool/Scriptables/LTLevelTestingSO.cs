using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(menuName = "Scriptables/LevelTool/LevelTestingScriptable")]
public class LTLevelTestingSO : ScriptableObject
{
    [HideInInspector]
    public LTPathFinding savedPathfinding;
    
    [HideInInspector]
    public List<GameObject> goalsList;

    public void SetGoalsList(List<GameObject> goals)
    {
        goalsList = new List<GameObject>();
        goalsList = goals;
    }
}
