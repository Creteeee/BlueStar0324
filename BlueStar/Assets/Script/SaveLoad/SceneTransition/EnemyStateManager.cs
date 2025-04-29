using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager:Singleton<EnemyStateManager>
{
    public Dictionary<string, Vector3> EnemyPositions = new Dictionary<string, Vector3>();
    public Dictionary<string, Vector3> EnemyRotations = new Dictionary<string, Vector3>();
    public Dictionary<string, float> EnemyHealth = new Dictionary<string, float>();

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SaveEnemyState(string name, Vector3 position,Vector3 rotation, float health)
    {
        EnemyPositions[name] = position;
        EnemyRotations[name] = rotation;
        EnemyHealth[name] = health;
    }
    
    public void Clear()
    {
        EnemyPositions.Clear();
        EnemyRotations.Clear();
        EnemyHealth.Clear();
    }


}
