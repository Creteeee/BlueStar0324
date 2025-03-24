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
        Direction = -Emitter.BulletDir;
        Debug.Log("我是子弹脚本");
        float x = speed * Direction.x;
        float y = speed * Direction.y;
        Debug.Log("子弹方向为"+Direction);
        this.transform.position += new Vector3(x, y, 0);
       // this.transform.position += new Vector3(3, 3, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Enemy"))
        {
            Debug.Log("击中敌人了");
            Destroy(gameObject);
            EventCenter_BattleMode.NotifyHitEnemy();
        }
    }
}
