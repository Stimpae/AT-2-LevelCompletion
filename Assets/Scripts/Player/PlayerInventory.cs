using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    public List<Key> keyInventory = new List<Key>();
    
    public bool MatchThisID(int id)
    {
        foreach (var key in keyInventory)
        {
            if (key.id == id)
            {
                return true;
            }
        }
        return false;
    }
}
