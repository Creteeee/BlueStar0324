using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet_Emitter : MonoBehaviour
{
    public float speed = 0.08f;
    public float lifetime = 1.5f;
    private Vector3 dir;
    public GameObject vfx;
    private bool ishit = false;
    


    private void Start()
    {
        dir = UIManager_BattleMode.bulletDirection;
    }

    private void Update()
    {
        this.transform.position += speed * dir;
        if (this.CompareTag("Froze") && !ishit)
        {
            
            StartCoroutine(DelayedInstantiate(vfx,this.transform.position,lifetime));
            Destroy(this.gameObject,lifetime*1.1f);
        }
        else
        {
            Destroy(this.gameObject,lifetime*1.1f);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameObject vfxInst;
            vfxInst=Instantiate(vfx,this.transform.position,Quaternion.identity);
            Destroy(this.gameObject);
            ishit = true;
            
        }
        
    }
    
    IEnumerator DelayedInstantiate(GameObject prefab, Vector3 position,float delay)
    {
        yield return new WaitForSeconds(delay);
        // 在延迟结束时再取位置
        Vector3 delayedPosition = this.transform.position;
        GameObject vfxInst;

        vfxInst=Instantiate(prefab, delayedPosition, quaternion.identity);
        vfxInst.transform.up=Vector3.forward;
        
    }
}

