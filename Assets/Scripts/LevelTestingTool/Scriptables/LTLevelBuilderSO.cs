using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Scriptables/LevelTool/LevelBuilderScriptable")]
public class LTLevelBuilderSO : ScriptableObject
{
    [Header("Building Objects")] 
    public GameObject wallObject;
    public GameObject floorObject;
    public GameObject keyObject;
    public GameObject doorObject;
    public GameObject startingObject;
    public GameObject endingObject;

    [Header("PlacementSettings")]
    public EObjectPlacementDirection placementDirection;
    public EObjectSelectionType placementType;
    
    public GameObject GetGameObjectFromEnum(EObjectSelectionType type)
    {
        switch (type)
        {
            case EObjectSelectionType.WALL:
                return wallObject;
            case EObjectSelectionType.FLOOR:
                return floorObject;
            case EObjectSelectionType.KEY:
                return keyObject;
            case EObjectSelectionType.DOOR:
                return doorObject;
            case EObjectSelectionType.START:
                return startingObject;
            case EObjectSelectionType.END:
                return endingObject;
        }
        return null;
    }

    public float GetValueFromDirectionEnum(EObjectPlacementDirection type)
    {
        switch (type)
        {
            case EObjectPlacementDirection.HORIZONTAL:
                return 0;
            case EObjectPlacementDirection.VERTICAL:
                return 90;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}

public enum EObjectPlacementDirection
{
    HORIZONTAL,
    VERTICAL,
}

public enum EObjectSelectionType
{
    WALL,
    FLOOR,
    KEY,
    DOOR,
    START,
    END
}
