using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    public GameObject BulletPos;
    public GameObject EndPoint;
    float range = 30.0f;
    public LineRenderer Line;
    public float Damage;
    public GameObject Blood;


    // Start is called before the first frame update
    void Start()
    {
        Line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1")) 
        {
            Shooting();
            StartCoroutine(ExampleCoroutine());
        }

    }


    public void Shooting() 
    
    {
        Line.SetPosition(0, BulletPos.transform.position);
        Line.SetPosition(1, EndPoint.transform.position);

        RaycastHit Rayhit;

        if (Physics.Raycast(BulletPos.transform.position, BulletPos.transform.forward, out Rayhit, range))
        {
            //  Debug.Log(hit.transform.name);


            if (Rayhit.transform.gameObject.tag == "Zombie")
            {
                Rayhit.transform.gameObject.SendMessage("EnemyDamage", Damage);
                Instantiate(Blood, Rayhit.point, Quaternion.LookRotation(Rayhit.normal));


            }
            



        }


    }

    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        Line.SetPosition(0, BulletPos.transform.position);
        Line.SetPosition(1, BulletPos.transform.position);

    }



    }
