using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnVolume : MonoBehaviour
{
    [Header("Volume Size")] 
    public Vector3 boxSize;

    [Header("References")] 
    public SpawnManager spawnManager;

    private BoxCollider _boxCollider;
    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        if (_boxCollider)
        {
            _boxCollider.size = boxSize;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            spawnManager.ResetPlayerPosition();
        }
    }
}
