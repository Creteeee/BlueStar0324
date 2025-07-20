using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryStateManager : Singleton<InventoryStateManager>
{
    public Dictionary<string, bool> DoorStates = new Dictionary<string, bool>();
    public Dictionary<string, int> ItemState = new Dictionary<string, int>(); //记录交互物体被访问的次数



    public void SaveDoorState(string name, bool canOpen)
    {
        DoorStates[name] = canOpen;
    }

    public void SaveItemState(string name, int times)
    {
        ItemState[name] = times;
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
