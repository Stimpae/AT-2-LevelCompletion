using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Object")] 
    public GameObject player;

    [Header("Location")] 
    public Transform spawnLocation;

    [Header("Cameras")] 
    public Camera mainCam;
    public List<CinemachineVirtualCamera> cmList;
    
    public static SpawnManager Instance { get; private set; }

    private GameObject m_player;

    private void Awake()
    {
        Instance = this;
        spawnLocation = GameObject.Find("StartLocation").GetComponent<Transform>();
    }
    
    public void SpawnPlayer(List<GameObject> goals)
    {
        Vector3 position = spawnLocation.position;
        position.y += 1.0f;
        
        m_player = Instantiate(player, position, spawnLocation.rotation);
        
        // sets the player goals
        m_player.GetComponent<LTPlayerController>().goalsList = goals;
        m_player.transform.SetParent(null);

        SetCameras();
    }

    private void SetCameras()
    {
        foreach (var camera in cmList)
        {
            CameraManager.Register(camera);
            if (player)
            {
                camera.m_LookAt = m_player.transform;
            }
        }
        CameraManager.SwitchCamera(cmList[0]);
    }

    public void ResetPlayerPosition()
    {
        m_player.transform.position = spawnLocation.position;
    }

}
