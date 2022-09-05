using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(LTLevelTool))]
public class LTLevelToolInspector : Editor
{
    // reference to our tool
    private LTLevelTool m_levelTool;
    
    // references to each of the editor windows
    private Editor m_builderEditor;
    private Editor m_testingEditor;
    
    private GameObject m_currentActiveGO;
    private float m_activeObjectYawRotation;
    private Vector3 m_hitPosition;
    
    // tab selection
    private string[] m_tabs = {"Level Builder", "Level Testing"};
    private int m_tabSelected = 0;

    // Handles the inspector update GUI
    public override void OnInspectorGUI()
    {
        m_levelTool = (LTLevelTool) target;
        
        // find the serialized data inside of our level tool class
        var buildData = serializedObject.FindProperty("levelBuilderSO");
        var testingData = serializedObject.FindProperty("levelTestingSO");
        
        // build an editor window for both.
        CreateCachedEditor(buildData.objectReferenceValue, this.GetType(), ref m_builderEditor);
        CreateCachedEditor(testingData.objectReferenceValue, this.GetType(), ref m_testingEditor);
        
        // update the objects
        serializedObject.Update();
        m_builderEditor.serializedObject.Update();
        m_testingEditor.serializedObject.Update();
        
        DrawPropertiesExcluding(serializedObject, "m_Script");
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Build Grid"))
        {
            if (m_levelTool)
            {
                Undo.RecordObject(Selection.activeGameObject, "BuiltLevel");
                m_levelTool.BuildLevelGrid();

                // Notice that if the call to RecordPrefabInstancePropertyModifications is not present,
                // all changes to scale will be lost when saving the Scene, and reopening the Scene
                // would revert the scale back to its previous value.
                PrefabUtility.RecordPrefabInstancePropertyModifications(Selection.activeGameObject);
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

                // Optional step in order to save the Scene changes permanently.
                //EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            }
        }
        
        EditorGUILayout.Space(10);
        
        serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.Space(10);
        
        // start drawing the tabs down here
        EditorGUILayout.BeginVertical();
        
        m_tabSelected = GUILayout.Toolbar(m_tabSelected, m_tabs);
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.Space(10);
        
        // handle the tab options
        HandleTabs();
        
        EditorGUILayout.Space(20);
    }

    // Handles the scene update GUI
    public void OnSceneGUI()
    {
        Event e = Event.current;

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (m_levelTool != null)
        {
            if (m_levelTool.m_pathfinding?.m_grid != null)
            {
                var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (!Physics.Raycast(ray, out var hit)) return;
                m_levelTool.SetHoverTarget(hit.transform.gameObject);

                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    Ray placementRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit placementHit;
                    if (!Physics.Raycast(placementRay, out placementHit)) return;
                    
                    EObjectSelectionType placementType = m_levelTool.levelBuilderSO.placementType;
                    GameObject go = m_levelTool.levelBuilderSO.GetGameObjectFromEnum(placementType);
                    Vector3 defaultEulers = go.transform.eulerAngles;

                    GameObject instance = Instantiate(go, m_levelTool.m_levelParentObject.transform);

                    Vector2 position = m_levelTool.m_pathfinding.m_grid.GetXZPosition(hit.point);
                    instance.transform.position = new Vector3(position.x + 0.5f, -0.5f, position.y + 0.5f);

                    EObjectPlacementDirection directionType = m_levelTool.levelBuilderSO.placementDirection;
                    float yawRotation = m_levelTool.levelBuilderSO.GetValueFromDirectionEnum(directionType);
                    
                    instance.transform.rotation = Quaternion.Euler(defaultEulers.x, yawRotation, defaultEulers.z);
                    instance.tag = go.tag;
                    instance.name = go.name;

                    if (hit.transform.gameObject.CompareTag("Debug"))
                    {
                        // only delete debug hit
                        DestroyImmediate(hit.transform.gameObject);
                        
                    }
                    
                    // sets the node is walkable for walls and doors.
                    if (instance.CompareTag("Wall"))
                    {
                        // need to set this position to not walkable
                        m_levelTool.SetNodeWalkable(hit.point, false);
                    }
                }

                // this remove as node from the scene (makes it not walkable)
                if (e.type == EventType.MouseDown && e.button == 1)
                {
                    Ray placementRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit placementHit;
                    if (!Physics.Raycast(placementRay, out placementHit)) return;
                    
                    if (hit.transform.gameObject.CompareTag("Debug"))
                    {
                        // only delete debug hit
                        DestroyImmediate(hit.transform.gameObject);
                        m_levelTool.SetNodeWalkable(hit.point, false);
                    }
                    
                }
            }
        }
        }
        
        SceneView.RepaintAll();
    }

    private void HandleTabs()
    {
        // dependent on the tabs switch between each method
        if (m_tabSelected >= 0)
        {
            switch (m_tabs[m_tabSelected])
            {
                case "Level Builder":
                    LevelBuilderTab();
                    break;
                case "Level Testing":
                    LevelTestingTab();
                    break;
                default:
                    break;
            }
        }
    }

    private void LevelBuilderTab()
    {
        DrawPropertiesExcluding(m_builderEditor.serializedObject, "m_Script");
        Selection.activeGameObject = m_levelTool.gameObject;
        m_builderEditor.serializedObject.ApplyModifiedProperties();
    }

    private void LevelTestingTab()
    {
        DrawPropertiesExcluding(m_testingEditor.serializedObject, "m_Script");
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Apply Testing References"))
        {
            if (m_levelTool)
            {
                m_testingEditor.serializedObject.ApplyModifiedProperties();
                
                Undo.RecordObject(Selection.activeGameObject, "SaveRefs");
                
                m_levelTool.SetReferences();
                
                // Notice that if the call to RecordPrefabInstancePropertyModifications is not present,
                // all changes to scale will be lost when saving the Scene, and reopening the Scene
                // would revert the scale back to its previous value.
                PrefabUtility.RecordPrefabInstancePropertyModifications(Selection.activeGameObject);
                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());

                // Optional step in order to save the Scene changes permanently.
                //EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
            }
        }
        
        m_testingEditor.serializedObject.ApplyModifiedProperties();
    }
}
