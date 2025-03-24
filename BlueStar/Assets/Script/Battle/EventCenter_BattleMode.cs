using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventCenter_BattleMode
{

    public static event Action OnLaunchEmitter;

    public static void NotifyLaunchEmitter()
    {
        OnLaunchEmitter?.Invoke();
    }

    public static event Action<bool> OnActivateManufactureUI;

    public static void NotifyManufactureUIActivated(bool isMaufactureUIActivated)
    {
        OnActivateManufactureUI?.Invoke(isMaufactureUIActivated);
    }

    public static event Action<bool> OnActivateActivateSpaceBattleUI;

    public static void NotifySpaceBattleUIActivated(bool isSpaceBattleUIActivated)
    {
        OnActivateManufactureUI?.Invoke(isSpaceBattleUIActivated);
    }

    public static event Action<int> OnUnlaunchedEmitterUpdated;//通知新加的Emitter的EmitterType

    public static void NotifyUnlaunchedEmitterUpdated(int i)
    {
        if (DataManager.updateUnlaunchedEmitterType != 0)//避免销毁的时候再报告一次
        {

        }
        OnUnlaunchedEmitterUpdated?.Invoke(i);
        Debug.Log("有新的未发射的Emitter，它的EmitterType是"+DataManager.updateUnlaunchedEmitterType);
       
    }

    public static event Action HitEnemy;

    public static void NotifyHitEnemy()
    {
        HitEnemy?.Invoke();
    }
    
    

}