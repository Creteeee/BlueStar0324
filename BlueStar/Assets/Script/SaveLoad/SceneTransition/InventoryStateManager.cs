using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryStateManager : Singleton<InventoryStateManager>
{
    public Dictionary<string, bool> DoorStates = new Dictionary<string, bool>();



    public void SaveDoorState(string name, bool canOpen)
    {
        DoorStates[name] = canOpen;
    }
    
    private void OnEnable()
    {
        EventHandler.DestroyObject += onDestroyDontDestroyOnLoadObjects;
    }

    private void OnDisable()
    {
        EventHandler.DestroyObject -= onDestroyDontDestroyOnLoadObjects;
    }

    void onDestroyDontDestroyOnLoadObjects(bool isDestroy)
    {
        Destroy(this.gameObject);
    }
}
