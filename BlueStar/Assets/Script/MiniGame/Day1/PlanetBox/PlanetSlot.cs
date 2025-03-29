using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetSlot : MonoBehaviour
{
    public bool isRight = false;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlanetModel"))
        {
            Debug.Log("检测到星球");
            if (other.name==this.gameObject.name)
            {
                isRight = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlanetModel"))
        {
            Debug.Log("检测到星球");
            if (other.name==this.gameObject.name)
            {
                isRight = false;
            }
        }
    }
}
