using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LTUtilities : MonoBehaviour
{
    public static LTUtilities Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetMouseWorldPosition()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera is { })
        {
            var worldPosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            return worldPosition;
        }
        return Vector3.zero;
    }
}
