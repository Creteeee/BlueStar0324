using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Emitter : MonoBehaviour
{
    public float speed = 0.08f;
    public float lifetime = 1.5f;
    private Vector3 dir;


    private void Start()
    {
        dir = UIManager_BattleMode.bulletDirection;
    }

    private void Update()
    {
        this.transform.position += speed * dir;
        Destroy(this.gameObject,lifetime);

    }


}

