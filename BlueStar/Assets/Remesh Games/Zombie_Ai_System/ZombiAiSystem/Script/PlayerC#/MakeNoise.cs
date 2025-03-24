using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeNoise : MonoBehaviour
{

   public KeyCode NoiseKey;
   [HideInInspector]
   public bool Noise;
   float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0.0f) 
        {
            timer -= Time.deltaTime;
        
        }
     
        
        if (Input.GetKeyDown(NoiseKey) || Bullet_Weapon.isHitZombie == true) 
        {
            Noise = true;
            timer = 0.1f;
        }

        
        if(Noise == true && timer <= 0.0f) 
        {
        
            Noise = false;
        }
        
    }






}
