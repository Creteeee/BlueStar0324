using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryStateManager : Singleton<InventoryStateManager>
{
    public Dictionary<string, bool> DoorStates = new Dictionary<string, bool>();

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SaveDoorState(string name, bool canOpen)
    {
        DoorStates[name] = canOpen;
    }
}
