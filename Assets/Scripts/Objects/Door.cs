using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door")] 
    public int id;
    public float newHeight = 4f;
    public Texture2D texture;
    
    
    private PlayerInventory playerInventory;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // need to check if that ids of all the keys match the
            // this current door
            playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory.MatchThisID(this.id))
            {
                Debug.Log("Player Open");
                OpenDoor();
            }
            else
            {
                Debug.Log("No Key");
                //display something to say no key is active
            }
        }
    }
    
    public void OpenDoor()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        GUIContent content = new GUIContent();
        content.text = "Door: ID: " + id.ToString();

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;

        Handles.Label(transform.position, content,style);
    }
    
}
