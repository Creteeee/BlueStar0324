using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{   
    public float speed;
    public float reflectionTimes; //可以反射的总次数
    public int reflectedTimes ;//已经反射的次数
    new public Rigidbody rigidBody;
    public GameObject explodePrefab;
    public Vector3 direction;
    private Renderer bulletRenderer;
    public bool isSplit;

  
    

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
       
    }

    void Update()
    {
        //子弹超出视野外，被销毁
  
        Fly(direction);
        bool isOut = Physics.Raycast(transform.position, direction, 0.6f, LayerMask.GetMask("border"));
        if (isOut)
        {
            Destroy(gameObject);
        }        
        Collision();

       
    }

    public void Fly(Vector3 dir)
    {
        if (reflectedTimes >= reflectionTimes)
        {
            Instantiate(explodePrefab, transform.position, Quaternion.identity);  
            Destroy(gameObject);
        }
       // rigidBody.velocity = dir * speed;
        this.transform.position += dir * speed*0.1f;
    }
    
    

    private void Collision()
    {
        int index = LayerMask.GetMask("obstacle");
        RaycastHit hit;
        bool ishit = Physics.Raycast(transform.position, direction, out hit,0.6f, index);
        if (ishit)
        {
            Vector3 hitPoint = hit.point;
            Vector3 hitNormal = hit.normal;
            Vector3 refDir = Vector3.Reflect(direction, hitNormal);
            reflectedTimes++;
            direction = refDir;
            if (isSplit)
            {
             CreateSplitBullets(direction,15f,hitPoint);   
            }
            
            //Destroy(gameObject);
           
        }
    }

    private void CreateSplitBullets(Vector3 direction, float splitAngle,Vector3 hitPoint)
    {
        //两个子弹的方向
        Vector3 dir1 = Quaternion.AngleAxis(splitAngle, Vector3.up) * direction;
        Vector3 dir2 = Quaternion.AngleAxis(-splitAngle, Vector3.up) * direction;
        GameObject bullet1 = Instantiate(this.gameObject, transform.position, Quaternion.identity);
        Bullet bulletScript1 = bullet1.GetComponent<Bullet>();
        bulletScript1.direction = dir1;
        GameObject bullet2 = Instantiate(this.gameObject, transform.position, Quaternion.identity);
        Bullet bulletScript2 = bullet2.GetComponent<Bullet>();
        bulletScript2.direction = dir2;

    }

}
