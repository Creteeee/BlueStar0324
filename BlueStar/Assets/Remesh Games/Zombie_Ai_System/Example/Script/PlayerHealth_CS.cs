using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth_CS : MonoBehaviour
{

    public float Health = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public void ApplyDamage(float Damage)
    {
        Health -= Damage;
    }



}
