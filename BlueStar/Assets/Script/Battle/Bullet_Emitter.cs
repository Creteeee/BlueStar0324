using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Emitter : MonoBehaviour
{
    public float speed = 0.08f;
    public Vector3 Direction = new Vector3(1,1,0);
    public List<GameObject> enemies;

    private void Start()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            string nameToSearch = "Enemy";  
            if (obj.name.Contains(nameToSearch))
            {
                enemies.Add(obj);
            }
        }
        
        
    }

    private void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

    }
}
