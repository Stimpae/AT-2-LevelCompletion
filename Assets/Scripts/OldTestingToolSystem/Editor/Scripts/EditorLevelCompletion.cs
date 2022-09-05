using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using Object = System.Object;

[CustomEditor(typeof(EditorCompletionHelper))]
public class EditorLevelCompletion : Editor
{
    public EditorCompletionHelper helper
    {
        get { return (EditorCompletionHelper) target; }
    }
    private bool _completionActive = true;
    private bool _creationActive = false;

    private bool _outputLogActive = false;
    private bool _debugActive = true;
    private Vector2 _outputScrollPosition;
    private  Vector2 _objectScrollPosition;

    private Rect _toolRect;
    private Rect _creationRect;
    private Rect _completabilityRect;
    private Rect _outputRect;
    private Rect _resultsRect;

    private List<GameObject> _sceneObjects = new List<GameObject>();
    private List<LogMessage> _logMessages = new List<LogMessage>();

    private void OnEnable()
    {
        EditorWindow.focusedWindow.Focus();
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.CompareTag("Door") || go.CompareTag("Key") ||
                go.CompareTag("EndVolume"))
            {
                _sceneObjects.Add(go);
                AddOutputMessage(LogLevel.MESSAGE_STANDARD, "Added -" + go.tag + " to steps list");
            }
        }
    }
    
    private void OnSceneGUI()
    {
        Handles.BeginGUI();
        Selection.activeObject = helper.gameObject;
        
        Debug.Log(LTUtilities.Instance.GetMouseWorldPosition());
        
        Rect sceneView = SceneView.lastActiveSceneView.position;
        _toolRect = new Rect(10, sceneView.yMin - 45, 190, 65);
        _creationRect = new Rect(10, sceneView.yMin + 30, 190, 65);
        _completabilityRect = new Rect(10, sceneView.yMin + 30, 210, 300);
        _outputRect = new Rect(10, sceneView.yMax - 250, sceneView.width - 20, 160);
        _resultsRect = new Rect(sceneView.xMax / 2, sceneView.yMin + 200, 200, 200);
        GUI.Window(0, _toolRect, LevelToolWindow, "Level Tool", "Window");
        if (_creationActive)
        {
            _outputLogActive = false;
            _completionActive = false;
            GUI.Window(1, _creationRect, LevelCreationWindow, "", GUIStyle.none);
        }
        
        if (_completionActive)
        {
            _creationActive = false;
            GUI.Window(2, _completabilityRect, LevelCompletionWindow, "Level Completion Tool", "Window");
        }

        if (_outputLogActive)
        {
            GUI.Window(3, _outputRect, OutPutWindow, "", GUIStyle.none);
        }
        
        if (helper.success)
        {
            GUI.Window(4, _resultsRect, LevelSuccessWindow, "Level Success");
        }

        if (helper.failure)
        {
            GUI.Window(5, _resultsRect, LevelFailureWindow, "Level Failure");
        }
        
        Handles.EndGUI();
    }

    private void LevelToolWindow(int id)
    {
        _creationActive = GUILayout.Toggle(_creationActive, "Level Creation Tool");
        _completionActive = GUILayout.Toggle(_completionActive, "Level Completability Tool");
    }

    private void LevelSuccessWindow(int id)
    {
        GUILayout.Label("The level is completable");
        if (GUILayout.Button("Reset Level", "Button"))
        {
            helper.Reset();
            foreach (var go in _sceneObjects)
            {
                go.gameObject.SetActive(true);
            }
            // this will have to wait for now
        }
    }

    private void LevelFailureWindow(int id)
    {
        GUIStyle myCustomStyle = new GUIStyle(GUI.skin.GetStyle("label"))
        {
            wordWrap = true
        };

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green;
        style.wordWrap = true;
        
        Rect labelRect = new Rect(10f, 20f, 190f, 40f);
        Rect labelRect2 = new Rect(10f, 80f, 190f, 40f);
        Rect labelRect3 = new Rect(10f, 110f, 190f, 40f);
        EditorGUI.LabelField(labelRect, helper.suggestions, EditorStyles.wordWrappedLabel);
        GUILayout.Space(35);
        GUILayout.Label("Suggestions");
        GUILayout.Space(5);
        EditorGUI.LabelField(labelRect2, helper.newSuggestions1, style);
        GUILayout.Space(5);
        EditorGUI.LabelField(labelRect3, helper.newSuggestions2, style);

        GUILayout.Space(60);
        if (GUILayout.Button("Reset Level", "Button"))
        {
            helper.Reset();
            foreach (var go in _sceneObjects)
            {
                go.gameObject.SetActive(true);
            }
        }
        
        if (GUILayout.Button("Implement Suggestions", "Button"))
        {
            // this will have to wait for now
        }
    }

    private void LevelCreationWindow(int id)
    {
        
    }
    
    private void AddOutputMessage(LogLevel level, string message)
    {
        LogMessage m;
        m.message = message;
        m.level = level;
        _logMessages.Add(m);
    }

    private void OutPutWindow(int id)
    {
        Rect sceneView = SceneView.lastActiveSceneView.position;
        
        GUILayout.BeginVertical("Output Log","Box");
        
        GUILayout.Space(20);
        GUILayout.BeginVertical(GUIStyle.none, GUILayout.Width(200), GUILayout.Height(160));
        _outputScrollPosition = GUILayout.BeginScrollView(_outputScrollPosition, false, 
            true, GUILayout.Width(sceneView.width - 35), GUILayout.Height(140));
      
        for (int i = 0; i < _logMessages.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_logMessages[i].level.ToString());
            GUILayout.Label(_logMessages[i].message);
            GUILayout.Space(sceneView.width - 400);
            GUILayout.EndHorizontal();
        }
        
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        
        GUILayout.EndVertical();
    }

    private void LevelCompletionWindow(int id)
    {
        _outputLogActive = GUILayout.Toggle(_outputLogActive, "Enable Debug Log");
        _debugActive = GUILayout.Toggle(_debugActive, "Enable Debugging Gizmos");
        
        GUILayout.Space(20);
        _objectScrollPosition = GUILayout.BeginScrollView(_objectScrollPosition, false, true, GUILayout.Width(200), GUILayout.Height(160));
        for (int i = 0; i < _sceneObjects.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_sceneObjects[i].tag);
            _sceneObjects[i] = (GameObject)EditorGUILayout.ObjectField(_sceneObjects[i], typeof(GameObject), true, 
                GUILayout.Width(100), GUILayout.Height(20));
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.Space(20);

        if (GUILayout.Button("Automate Steps", "Button"))
        {
            // this will have to wait for now
        }
        
        if (GUILayout.Button("Attempt Level", "Button"))
        {
            EditorCoroutineUtility.StartCoroutine(helper.AttemptSteps(_sceneObjects), this);
            
            // if not a success then show window + output log.
            //get a reference to the current enviroment
            //implement some of the unit stuff in here -> but will have to do additional checks
        }
        //buttons 3

        //loop through the number of things in the count
        //create the correct fields
        //assign automatic?


        // need an output log window
        // need a window that shows all of the objects and steps (can fill in steps manually)
        // need a button for automatically generate steps
        // button for attempting level -> this prints to the output log window
    }
}

public enum LogLevel
{
    MESSAGE_STANDARD,
    MESSAGE_WARNING,
    MESSAGE_FAILED
}

public struct LogMessage
{
    public LogLevel level;
    public string message;
}
