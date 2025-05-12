using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgressManager : Singleton<GameProgressManager>
{
    [Header("Day2_Zombie_Die")]
    public bool Day2_Zombie_isAlive = true;
    [Header("Day2_Work")]
    public bool Day2_Work_isFinished=false;
    public GameObject Timeline_UnknownPersonLeaveSuggest;
    
    

    public void Day2_Work_Finished()
    {
        if (Day2_Work_isFinished)
        {
            Timeline_UnknownPersonLeaveSuggest.SetActive(true);
            InventoryStateManager.Instance.DoorStates["Door_ControlRoom_wing"]=true;
            InventoryStateManager.Instance.DoorStates["Door_TrainingCourse_To_Decompression_wing"]=true;
            InventoryStateManager.Instance.DoorStates["Door_Corridor_1_wing"]=true;
        }
    }

    public void Day2_Zombie_Die()
    {
        
    }
}
