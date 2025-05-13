using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLine : MonoBehaviour
{
    public float speed=1f;
 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
    }
}
