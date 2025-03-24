using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Weapon : MonoBehaviour
{
    private float lifetime = 0.5f;  // 子弹销毁时间
    private float timeElapsed = 0f;  // 计时器
    public GameObject bloodVFX;
    private GameObject bloodVFXInst;
    public static bool isHitZombie=false;
    public float Damage = 20;



    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= lifetime)
        {
            Destroy(gameObject);  // 销毁子弹对象
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))  // 按下鼠标左键
        {

        }

        this.transform.position += transform.forward * 1;  // 子弹前进
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Bullet hit"+other.gameObject.name);
        bloodVFXInst = Instantiate(bloodVFX, this.transform.position, Quaternion.identity);
        bloodVFXInst.transform.localScale *= 1f;
        Destroy(bloodVFXInst ,1f);


        // 如果碰撞的是敌人，销毁子弹
        if (other.gameObject.CompareTag("Zombie"))
        {
            Debug.Log("Bullet hit an Zombie!");
            isHitZombie = true;
            other.gameObject.SendMessage("EnemyDamage",Damage);
            Destroy(this.gameObject);  // 销毁子弹
        }
    }
}
