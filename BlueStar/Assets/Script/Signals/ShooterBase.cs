using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBase : MonoBehaviour
{
    private Vector3 direction;


    private GameObject shooter;
    // Start is called before the first frame update
    void Start()
    {
        shooter = GameObject.Find("shootPos");
        
 
    }

    // Update is called once per frame
    void Update()
    {
        direction = shooter.GetComponent<Shooter>().direction;
        transform.forward = direction;
    }
}
