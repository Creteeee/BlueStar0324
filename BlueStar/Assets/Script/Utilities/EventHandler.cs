using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;

    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location,list);
    }

    public static event Action<int> ResetEmptySlot;

    public static void CallResetEmptySlot(int index)
    {
        ResetEmptySlot?.Invoke(index);
    }
    
    public static event Action<bool,int,string[]> ShowExpectItemUI;

    public static void CallShowExpectedItemUI(bool isInteracted,int ID,string[] informations)
    {
        ShowExpectItemUI?.Invoke(isInteracted,ID,informations);
    }
    

    public static event Action<string[]> PassObserveInfos;
    public static void CallPassObserveInfos(string[] infos)
    {
        PassObserveInfos?.Invoke(infos);
    }

    public static event Action<bool> MoveHeader;

    public static void CallMoveHeader(bool isMove)
    {
        MoveHeader?.Invoke(isMove);
    }
    
    public static event Action<bool> ResetHeader;

    public static void CallResetHeader(bool isEnded)
    {
        ResetHeader?.Invoke(isEnded);
    }

    public static event Action<string[], int[]> ShowTimelineInfos;
    
/// <summary>
/// Timeline动画底端的字幕
/// </summary>
/// <param name="infos"></param>
/// <param name="playIndexs"></param>
    public static void CallShowTimelineInfos(string[] infos, int[] playIndexs)
    {
        ShowTimelineInfos?.Invoke(infos,playIndexs);
    }

}
