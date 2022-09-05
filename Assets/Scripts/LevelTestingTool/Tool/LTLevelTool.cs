using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

[ExecuteAlways]
[Serializable]
public class LTLevelTool : MonoBehaviour
{
    [HideInInspector]
    [SerializeField]public LTPathFinding m_pathfinding;
    
    [Header("Default Settings")]
    [SerializeField] public GameObject m_levelParentObject;
    [SerializeField] private LTLevelBuilderSO m_builderDefaults;
    [SerializeField] private LTLevelTestingSO m_testingDefaults;
    [SerializeField] private int m_levelWidthHeight;

    [Header("Debug Settings")] 
    [SerializeField] private GameObject m_levelDebugObject;
    [SerializeField] private Material m_defaultDebugMaterial;
    [SerializeField] private Material m_selectedDebugMaterial;

    // contains all of the data related to our pathfinding and level building
    [HideInInspector] public LTLevelBuilderSO levelBuilderSO;
    [HideInInspector] public LTLevelTestingSO levelTestingSO;

    // grid and level scriptable object
    // level testing scriptable object
    // Start is called before the first frame update

    private GameObject m_currentHoverTarget;
    private List<GameObject> m_debugGameObjects = new List<GameObject>();
    [SerializeField] private List<GameObject> m_goalsList = new List<GameObject>();
    private LTPathRequestManager m_pathRequestManager;

    public void OnEnable()
    {
        levelBuilderSO = ScriptableObject.CreateInstance<LTLevelBuilderSO>();
        levelTestingSO = ScriptableObject.CreateInstance<LTLevelTestingSO>();
        levelBuilderSO = m_builderDefaults;
        levelTestingSO = m_testingDefaults;
    }

    [ContextMenu("Build Grid Debug")]
    public void BuildLevelGrid()
    {
        m_pathfinding = GetComponent<LTPathFinding>();
        m_pathfinding.InitialisePathFinding(m_levelWidthHeight,m_levelWidthHeight,m_pathRequestManager);
        m_testingDefaults.savedPathfinding = m_pathfinding;
        
        m_debugGameObjects = new List<GameObject>();
        
        // gets all of the children in the current level and destroys them
        // build grid should only be used once to have a visual representation of the editor.
        var children = m_levelParentObject.transform.childCount;
        if (children > 0)
        {
            for (int i = children - 1; i >= 0; i--)
            {
                DestroyImmediate(m_levelParentObject.transform.GetChild(i).gameObject);
            }
        }

        var debugChildren = m_levelDebugObject.transform.childCount;
        if (debugChildren > 0)
        {
            for (int i = debugChildren - 1; i >= 0; i--)
            {
                DestroyImmediate(m_levelDebugObject.transform.GetChild(i).gameObject);
            }
        }

        // for each node we then need to build a debug space for it
        foreach (var node in m_pathfinding.m_grid.gridArray)
        {
            GameObject go = node.DebugNodeMesh(m_levelDebugObject);
            go.GetComponent<MeshRenderer>().sharedMaterial = m_defaultDebugMaterial;
            go.transform.position = node.worldPosition;
            m_debugGameObjects.Add(go);
        }
    }
    
    public void SetNodeWalkable(Vector3 hitWorldPosition,bool isWalkable)
    {
        var xy = m_pathfinding.m_grid.GetXZPosition(hitWorldPosition);
        var node = m_pathfinding.m_grid.GetNodeAtWorldPosition(xy);
        node.isWalkable = isWalkable;
    }
    
    public void SetHoverTarget(GameObject hitTarget)
    {
        if (hitTarget.CompareTag("Debug"))
        {
            hitTarget.GetComponent<MeshRenderer>().sharedMaterial = m_selectedDebugMaterial;
            GameObject targetObject = hitTarget;
        
            if (targetObject != null)
            {
                if (m_currentHoverTarget != targetObject)
                {
                    if (m_currentHoverTarget != null)
                    {
                        m_currentHoverTarget.GetComponent<MeshRenderer>().sharedMaterial = m_defaultDebugMaterial;
                    }

                    targetObject.GetComponent<MeshRenderer>().sharedMaterial = m_selectedDebugMaterial;
                    m_currentHoverTarget = targetObject;
                }
            }
            else if (m_currentHoverTarget != null)
            {
                m_currentHoverTarget.GetComponent<MeshRenderer>().sharedMaterial = m_defaultDebugMaterial;
                m_currentHoverTarget = null;
            }
        }
    }

    public void SetReferences()
    {
        m_goalsList = levelTestingSO.goalsList;
        m_testingDefaults.SetGoalsList(levelTestingSO.goalsList);
        m_testingDefaults.savedPathfinding = m_pathfinding;
    }
    
    
    public void Start()
    {
        m_pathRequestManager = GetComponent<LTPathRequestManager>();
        m_pathfinding = GetComponent<LTPathFinding>();
        
        m_pathRequestManager.InitiatePathFindingManager(m_pathfinding);
        m_pathfinding.m_requestManager = m_pathRequestManager;

        levelTestingSO.goalsList = m_testingDefaults.goalsList;
        m_goalsList = m_testingDefaults.goalsList;
        SpawnManager.Instance.SpawnPlayer(m_goalsList);
    }
    
}