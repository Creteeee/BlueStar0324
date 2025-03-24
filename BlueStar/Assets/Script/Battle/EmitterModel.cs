using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitterModel : MonoBehaviour
{
    public GameObject[] bullets;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bullets[Emitter.currentBulletID].SetActive(false);
        
    }
}
