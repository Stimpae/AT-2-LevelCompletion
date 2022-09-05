using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Key : MonoBehaviour
{
    [Header("Key")]
    public int id;

    private PlayerInventory playerInventory;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInventory = other.GetComponent<PlayerInventory>();
            playerInventory.keyInventory.Add(this);
            gameObject.SetActive(false);
        }
    }
    
    private void OnDrawGizmos()
    {
        GUIContent content = new GUIContent();
        content.text = "Key: ID: " + id.ToString();

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;

        Handles.Label(transform.position, content,style);
    }
}
