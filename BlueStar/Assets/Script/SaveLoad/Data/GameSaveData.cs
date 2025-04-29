using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  BlueStar.Save
{
    [System.Serializable]
    public class GameSaveData 
    {
        //敌人的位置
        public Dictionary<string, Vector3> enemyPosDict;
    }
    
    
}

